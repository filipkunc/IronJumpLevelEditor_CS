using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.VisualTree;
using IronJumpAvalonia.Game;
using System.Xml.Linq;
using Avalonia.Diagnostics;
using System.Diagnostics;
using System.IO;

namespace IronJumpAvalonia.Controls
{
	public class LevelEditor : Control
	{
		public enum FPDragHandle
		{
			None = 0,

			TopLeft,
			BottomLeft,
			TopRight,
			BottomRight,

			MiddleLeft,
			MiddleTop,
			MiddleRight,
			MiddleBottom,

			Center
		}

		UndoManager _undoManager = new UndoManager();

		Brush _backBrush = new SolidColorBrush(Color.FromRgb(55, 60, 89));
		Pen _gridPen = new Pen(new SolidColorBrush(Color.FromArgb((int)(0.2 * 255), 255, 255, 255)));
		Brush _selectionBrush = new SolidColorBrush(Color.FromArgb(50, 255, 255, 255));
		Pen _selectionPen = new Pen(new SolidColorBrush(Color.FromArgb(230, 255, 255, 255)));

		Rect ClippedBounds => new Rect(-Bounds.Position, this.GetTransformedBounds()!.Value.Clip.Size);

		bool _drawingSelection = false;
		Point _beginSelection;
		Point _endSelection;

		Rect SelectionRect => new Rect(
			new Point(Math.Min(_beginSelection.X, _endSelection.X) + 0.5, Math.Min(_beginSelection.Y, _endSelection.Y) + 0.5),
			new Point(Math.Max(_beginSelection.X, _endSelection.X) + 0.5, Math.Max(_beginSelection.Y, _endSelection.Y) + 0.5)
		);

		List<FPGameObject> _gameObjects = new List<FPGameObject>();
		SortedSet<int> _selectedIndices = new SortedSet<int>();

		Point _beginMovePoint;
		Point _endMovePoint;
		FPDragHandle _currentHandle = FPDragHandle.None;

		public ListBox FactoryView {  get; set; }

		List<Func<FPGameObject>> _factories = new List<Func<FPGameObject>>
		{
			() => new FPPlayer(),
			() => new FPPlatform(),
			() => new FPMovablePlatform(),
			() => new FPElevator(),
			() => new FPDiamond(),
			() => new FPMagnet(),
			() => new FPSpeedPowerUp(),
			() => new FPTrampoline(),
			() => new FPExit(),
		};

		Func<FPGameObject> SelectedFactory
		{
			get
			{
				if (FactoryView.SelectedIndex >= 0 && FactoryView.SelectedIndex < _factories.Count)
					return _factories[FactoryView.SelectedIndex];
				return null;
			}
			set
			{
				if (_previousObjects != null)
					AfterAction("Add New Object");
				FactoryView.SelectedIndex = -1;
				FactoryView.InvalidateVisual();
			}
		}

		void DrawGrid(DrawingContext context)
		{
			var rect = ClippedBounds;

			int offsetX = (int)(rect.X / 32) * 32;
			int offsetY = (int)(rect.Y / 32) * 32;
			rect = rect.WithX(offsetX).WithY(offsetY);

			for (int y = (int)rect.Top; y < (int)rect.Bottom + 32; y += 32)
				context.DrawLine(_gridPen, new Point(rect.Left, y + 0.5), new Point(rect.Right + 32, y + 0.5));

			for (int x = (int)rect.Left; x < (int)rect.Right + 32; x += 32)
				context.DrawLine(_gridPen, new Point(x + 0.5, rect.Top), new Point(x + 0.5, rect.Bottom + 32));
		}

		protected override void OnLoaded(RoutedEventArgs e)
		{
			base.OnLoaded(e);
			Parent!.PropertyChanged += Parent_PropertyChanged;
			InitAllTextures();
		}

		private void InitAllTextures()
		{
			FPGame.InitTexture();
			FPPlayer.InitTextures();
			FPPlatform.InitTextures();
			FPDiamond.InitTextures();
			FPElevator.InitTextures();
			FPMovablePlatform.InitTextures();
			FPMagnet.InitTextures();
			FPSpeedPowerUp.InitTextures();
			FPTrampoline.InitTextures();
			FPExit.InitTextures();
		}

		public void LoadLevel(string fileContent)
		{
			_undoManager.Clear();
			_previousObjects = null;
			_previousIndices = null;
			_gameObjects.Clear();
			SelectedFactory = null;

			XElement root = XElement.Parse(fileContent);
			foreach (var element in root.Elements())
			{
				var type = Type.GetType("IronJumpAvalonia.Game." + element.Name.ToString());
				var gameObject = (FPGameObject)Activator.CreateInstance(type);
				gameObject.InitFromElement(element);
				_gameObjects.Add(gameObject);
				if (gameObject.NextPart != null)
					_gameObjects.Add(gameObject.NextPart);
			}

			InvalidateVisual();
		}

		public void SaveLevel(Stream stream)
		{
			XElement root = new XElement("IronJumpLevel");
			foreach (var gameObject in _gameObjects)
			{
				if (gameObject is FPElevatorEnd)
					continue;

				XElement gameObjectElement = new XElement(gameObject.GetType().Name);
				gameObject.WriteToElement(gameObjectElement);
				root.Add(gameObjectElement);
			}
			root.Save(stream);
			_undoManager.DocumentSaved();
		}

		public void Play()
		{
			var gameWindow = new GameWindow();
			gameWindow.Width = 480;
			gameWindow.Height = 320;
			gameWindow.Game = new FPGame(480, 320, _gameObjects);
			gameWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			gameWindow.Show();
		}

		private void Parent_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
		{
			if (e.Property == BoundsProperty)
			{
				InvalidateVisual();
			}
		}

		protected override void OnPointerPressed(PointerPressedEventArgs e)
		{
			base.OnPointerPressed(e);

			var location = e.GetPosition(this);
			var flags = e.KeyModifiers;


			if (_currentHandle != FPDragHandle.None && _currentHandle != FPDragHandle.Center)
			{
				BeginResize(_currentHandle);
				return;
			}

			if (SelectedFactory != null)
			{
				var draggedObject = SelectedFactory();
				int x = (int)location.X;
				int y = (int)location.Y;
				x /= 32;
				x *= 32;
				y /= 32;
				y *= 32;
				_beginMovePoint = _beginMovePoint.WithX(x).WithY(y);
				_endMovePoint = _beginMovePoint;
				draggedObject.Move(x, y);
				AddNewGameObject(draggedObject);
			}
			else
			{
				bool startSelection = true;

				if (flags.HasFlag(KeyModifiers.Control))
				{
					for (int i = 0; i < _gameObjects.Count; i++)
					{
						var gameObject = _gameObjects[i];
						if (gameObject.Rect.Contains(location))
						{
							if (_selectedIndices.Contains(i))
								_selectedIndices.Remove(i);
							else
								_selectedIndices.Add(i);

							startSelection = false;
							break;
						}
					}
				}
				else if (flags.HasFlag(KeyModifiers.Shift))
				{
					for (int i = 0; i < _gameObjects.Count; i++)
					{
						var gameObject = _gameObjects[i];
						if (gameObject.Rect.Contains(location))
						{
							_selectedIndices.Add(i);
							startSelection = false;
							break;
						}
					}
				}
				else if (_currentHandle == FPDragHandle.None)
				{
					for (int i = 0; i < _gameObjects.Count; i++)
					{
						var gameObject = _gameObjects[i];
						if (gameObject.Rect.Contains(location))
						{
							_selectedIndices.Clear();
							_selectedIndices.Add(i);
							startSelection = false;
							break;
						}
					}
				}
				else
				{
					startSelection = false;
					BeginMove();
				}

				if (startSelection)
				{
					_drawingSelection = true;
					_beginSelection = _endSelection = location;
				}
			}
			InvalidateVisual();
		}

		protected override void OnPointerMoved(PointerEventArgs e)
		{
			base.OnPointerMoved(e);
			var currentPoint = e.GetCurrentPoint(this);
			var location = e.GetPosition(this);
			if (currentPoint.Properties.IsLeftButtonPressed)
			{
				if (_drawingSelection)
				{
					_endSelection = location;
					InvalidateVisual();
				}
				else if (_currentHandle != FPDragHandle.None)
				{
					switch (_currentHandle)
					{
						case FPDragHandle.TopLeft:
							ResizeDraggedObjectTop((float)location.Y);
							ResizeDraggedObjectLeft((float)location.X);
							break;
						case FPDragHandle.TopRight:
							ResizeDraggedObjectTop((float)location.Y);
							ResizeDraggedObjectRight((float)location.X);
							break;
						case FPDragHandle.BottomLeft:
							ResizeDraggedObjectBottom((float)location.Y);
							ResizeDraggedObjectLeft((float)location.X);
							break;
						case FPDragHandle.BottomRight:
							ResizeDraggedObjectBottom((float)location.Y);
							ResizeDraggedObjectRight((float)location.X);
							break;
						case FPDragHandle.MiddleTop:
							ResizeDraggedObjectTop((float)location.Y);
							break;
						case FPDragHandle.MiddleBottom:
							ResizeDraggedObjectBottom((float)location.Y);
							break;
						case FPDragHandle.MiddleLeft:
							ResizeDraggedObjectLeft((float)location.X);
							break;
						case FPDragHandle.MiddleRight:
							ResizeDraggedObjectRight((float)location.X);
							break;
						case FPDragHandle.Center:
							if (_selectedIndices.Count == 1)
								MoveDraggedObject((float)location.X, (float)location.Y);
							else
								MoveSelectedObjects((float)location.X, (float)location.Y);
							break;
						default:
							break;
					}
					InvalidateVisual();
				}
				else if (SelectedFactory != null)
				{
					int widthSegments = (int)((location.X - _endMovePoint.X + 16.0f) / 32.0f);
					int heightSegments = (int)((location.Y - _endMovePoint.Y + 16.0f) / 32.0f);

					var draggedObjectLocation = _endMovePoint;
					if (widthSegments < 0)
						draggedObjectLocation = draggedObjectLocation.WithX(draggedObjectLocation.X + widthSegments * 32.0f);
					if (heightSegments < 0)
						draggedObjectLocation = draggedObjectLocation.WithY(draggedObjectLocation.Y + heightSegments * 32.0f);

					widthSegments = Math.Max(Math.Abs(widthSegments), 1);
					heightSegments = Math.Max(Math.Abs(heightSegments), 1);

					SetDraggedObjectPosition((float)draggedObjectLocation.X, (float)draggedObjectLocation.Y);
					var draggedObject = DraggedObject;

					draggedObject.WidthSegments = widthSegments;
					draggedObject.HeightSegments = heightSegments;
					InvalidateVisual();
				}
			}
			else
			{
				_currentHandle = FPDragHandle.None;

				const double handleSize = 14.0;
				var handleRect = new Rect(0.0, 0.0, handleSize, handleSize);

				if (_selectedIndices.Count == 1)
				{
					var draggedObject = DraggedObject;
					for (FPDragHandle handle = FPDragHandle.TopLeft; handle < FPDragHandle.Center; handle++)
					{
						if (!RespondsToDragHandle(draggedObject, handle))
							continue;

						var handlePoint = PointFromHandleAroundRect(handle, draggedObject.Rect);

						handleRect = handleRect
							.WithX(handlePoint.X - handleRect.Width / 2.0f)
							.WithY(handlePoint.Y - handleRect.Height / 2.0f);

						if (handleRect.Contains(location))
						{
							_currentHandle = handle;
							_beginMovePoint = _endMovePoint = location;
							break;
						}
					}

					if (_currentHandle == FPDragHandle.None)
					{
						if (draggedObject.Rect.Contains(location))
						{
							_currentHandle = FPDragHandle.Center;
							_beginMovePoint = _endMovePoint = location;
						}
					}
				}
				else
				{
					foreach (var index in _selectedIndices)
					{
						var gameObject = _gameObjects[index];
						if (gameObject.Rect.Contains(location))
						{
							_currentHandle = FPDragHandle.Center;
							_beginMovePoint = _endMovePoint = location;
							break;
						}
					}

				}
				InvalidateVisual();
			}
		}

		protected override void OnPointerReleased(PointerReleasedEventArgs e)
		{
			base.OnPointerReleased(e);
			if (_drawingSelection)
			{
				var selectionRect = SelectionRect;
				var flags = e.KeyModifiers;

				if (flags.HasFlag(KeyModifiers.Control))
				{
					for (int i = 0; i < _gameObjects.Count; i++)
					{
						var gameObject = _gameObjects[i];
						if (selectionRect.Intersects(gameObject.Rect))
						{
							if (_selectedIndices.Contains(i))
								_selectedIndices.Remove(i);
							else
								_selectedIndices.Add(i);
						}
					}
				}
				else
				{
					if (!flags.HasFlag(KeyModifiers.Shift))
						_selectedIndices.Clear();

					for (int i = 0; i < _gameObjects.Count; i++)
					{
						var gameObject = _gameObjects[i];
						if (selectionRect.Intersects(gameObject.Rect))
						{
							_selectedIndices.Add(i);
						}
					}
				}

				_drawingSelection = false;
			}
			else
			{
				var move = _endMovePoint.WithX(_endMovePoint.X - _beginMovePoint.X).WithY(_endMovePoint.Y - _beginMovePoint.Y);
				if (_currentHandle == FPDragHandle.Center)
					EndMove(move);
				else if (_currentHandle != FPDragHandle.None)
					EndResize(move);

				SelectedFactory = null;
			}
			InvalidateVisual();
		}

		private List<FPGameObject> _previousObjects = null;
		private SortedSet<int> _previousIndices = null;
		string _beforeActionName = null;
		private void RevertAction(string name, List<FPGameObject> objects, SortedSet<int> indices)
		{
			List<FPGameObject> copiedObjects = new List<FPGameObject>();
			SortedSet<int> copiedIndices = new SortedSet<int>();

			DuplicateCurrentObjectsAndIndices(copiedObjects, copiedIndices);

			_gameObjects.Clear();
			_gameObjects.AddRange(objects);
			_selectedIndices.Clear();
			_selectedIndices.AddRange(indices);

			_undoManager.PrepareUndo(name, Invocation.Create(name, copiedObjects, copiedIndices,
				(a, b, c) => RevertAction(a, b, c)));

			InvalidateVisual();
		}

		private void DuplicateCurrentObjectsAndIndices(List<FPGameObject> currentObjects, SortedSet<int> currentIndices)
		{
			currentObjects.Clear();

			foreach (var gameObject in _gameObjects)
			{
				var duplicate = gameObject.Duplicate(0.0f, 0.0f);
				if (duplicate != null)
				{
					currentObjects.Add(duplicate);
				}
				else // elevator end
				{
					var parentInGameObjects = gameObject.NextPart;
					var parentInOldObjects = currentObjects[_gameObjects.IndexOf(parentInGameObjects)];
					var nextPart = parentInOldObjects.NextPart;
					currentObjects.Add(nextPart);
				}
			}

			currentIndices.Clear();
			currentIndices.AddRange(_selectedIndices);
		}

		private void BeforeAction(string name)
		{
			if (_previousObjects != null || _previousIndices != null)
				throw new ApplicationException("BeforeAction called twice");

			_previousObjects = new List<FPGameObject>();
			_previousIndices = new SortedSet<int>();
			_beforeActionName = name;

			DuplicateCurrentObjectsAndIndices(_previousObjects, _previousIndices);
		}

		private void FullAction(string name, Action<LevelEditor> action)
		{
			string previousActionName = _beforeActionName;

			if (previousActionName != null)
				AfterAction(_beforeActionName);

			BeforeAction(name);
			action(this);
			AfterAction(name);

			if (previousActionName != null)
				BeforeAction(previousActionName);
		}

		private void AfterAction(string name)
		{
			_undoManager.PrepareUndo(name, Invocation.Create(name, _previousObjects, _previousIndices,
				(a, b, c) => RevertAction(a, b, c)));

			_previousObjects = null;
			_previousIndices = null;
			_beforeActionName = null;
		}

		private void AddNewGameObject(FPGameObject gameObject)
		{
			BeforeAction("Add New Object");
			_gameObjects.Add(gameObject);
			_selectedIndices.Clear();
			_selectedIndices.Add(_gameObjects.Count - 1);
			if (gameObject.NextPart != null)
				_gameObjects.Add(gameObject.NextPart);
		}

		private void BeginResize(FPDragHandle handle)
		{
			BeforeAction("Resize Selected");
		}

		void EndResize(Point move)
		{
			AfterAction("Resize Selected");
		}

		private void BeginMove()
		{
			BeforeAction("Move Selected");
		}

		void EndMove(Point move)
		{
			AfterAction("Move Selected");
		}

		private void DrawHandlesOnGameObject(DrawingContext context, FPGameObject gameObject)
		{
			var rect = gameObject.Rect;

			var whiteBrush = new SolidColorBrush(Color.FromArgb(204, 255, 255, 255));
			var redBrush = new SolidColorBrush(Color.FromArgb(204, 255, 0, 0));
			var yellowBrush = new SolidColorBrush(Color.FromArgb(204, 255, 255, 0));

			context.DrawRectangle(new Pen(whiteBrush), rect);

			var draggedObject = DraggedObject;

			bool widthHandles = false;
			bool heightHandles = false;

			if (draggedObject != null)
			{
				widthHandles = draggedObject.WidthSegments > 0;
				heightHandles = draggedObject.HeightSegments > 0;
			}

			for (FPDragHandle handle = FPDragHandle.TopLeft; handle < FPDragHandle.Center; handle++)
			{
				if (!widthHandles && IsWidthHandle(handle))
					continue;

				if (!heightHandles && IsHeightHandle(handle))
					continue;

				var handleBrush = handle == _currentHandle ? redBrush: yellowBrush;
				var handlePoint = PointFromHandleAroundRect(handle, rect);
				context.FillRectangle(handleBrush, new Rect(handlePoint.X - 3, handlePoint.Y - 3, 6, 6));
			}
		}

		FPGameObject DraggedObject
		{
			get
			{
				if (_selectedIndices.Count == 1)
					return _gameObjects[_selectedIndices.First()];
				return null;
			}
		}

		bool RespondsToDragHandle(FPGameObject gameObject, FPDragHandle dragHandle)
		{
			bool respondsToWidth = gameObject.WidthSegments > 0;
			bool respondsToHeight = gameObject.HeightSegments > 0;

			if (!respondsToWidth && IsWidthHandle(dragHandle))
				return false;

			if (!respondsToHeight && IsHeightHandle(dragHandle))
				return false;

			return true;
		}

		void SetDraggedObjectPosition(float x, float y)
		{
			var draggedObject = DraggedObject;
			draggedObject.Move(x - draggedObject.X, y - draggedObject.Y);
		}

		void ResizeDraggedObjectLeft(float x)
		{
			var draggedObject = DraggedObject;
			int widthSegments = (int)((x - _endMovePoint.X + 16.0f) / 32.0f);

			if (draggedObject.WidthSegments - widthSegments < 1)
				widthSegments = 0;

			draggedObject.Move(widthSegments * 32.0f, 0.0f);
			_endMovePoint = _endMovePoint.WithX(_endMovePoint.X + widthSegments * 32.0f);
			draggedObject.WidthSegments -= widthSegments;
		}

		void ResizeDraggedObjectRight(float x)
		{
			var draggedObject = DraggedObject;
			int widthSegments = (int)((x - _endMovePoint.X + 16.0f) / 32.0f);

			if (draggedObject.WidthSegments + widthSegments < 1)
				widthSegments = 0;

			_endMovePoint = _endMovePoint.WithX(_endMovePoint.X + widthSegments * 32.0f);
			draggedObject.WidthSegments += widthSegments;
		}

		void ResizeDraggedObjectTop(float y)
		{
			var draggedObject = DraggedObject;
			int heightSegments = (int)((y - _endMovePoint.Y + 16.0f) / 32.0f);

			if (draggedObject.HeightSegments - heightSegments < 1)
				heightSegments = 0;

			draggedObject.Move(0.0f, heightSegments * 32.0f);
			_endMovePoint = _endMovePoint.WithY(_endMovePoint.Y + heightSegments * 32.0f);
			draggedObject.HeightSegments -= heightSegments;
		}

		void ResizeDraggedObjectBottom(float y)
		{
			var draggedObject = DraggedObject;
			int heightSegments = (int)((y - _endMovePoint.Y + 16.0f) / 32.0f);

			if (draggedObject.HeightSegments + heightSegments < 1)
				heightSegments = 0;

			_endMovePoint = _endMovePoint.WithY(_endMovePoint.Y + heightSegments * 32.0f);
			draggedObject.HeightSegments += heightSegments;
		}

		void MoveDraggedObject(float x, float y)
		{
			var draggedObject = DraggedObject;
			int widthSegments = (int)((x - _endMovePoint.X + 16.0f) / 32.0f);
			int heightSegments = (int)((y - _endMovePoint.Y + 16.0f) / 32.0f);

			draggedObject.Move(widthSegments * 32.0f, heightSegments * 32.0f);
			_endMovePoint = _endMovePoint.WithX(_endMovePoint.X + widthSegments * 32.0f);
			_endMovePoint = _endMovePoint.WithY(_endMovePoint.Y + heightSegments * 32.0f);
		}

		void MoveSelectedObjects(float x, float y)
		{
			int widthSegments = (int)((x - _endMovePoint.X + 16.0f) / 32.0f);
			int heightSegments = (int)((y - _endMovePoint.Y + 16.0f) / 32.0f);

			foreach (var index in _selectedIndices)
			{
				var gameObject = _gameObjects[index];
				gameObject.Move(widthSegments * 32.0f, heightSegments * 32.0f);
			}

			_endMovePoint = _endMovePoint.WithX(_endMovePoint.X + widthSegments * 32.0f);
			_endMovePoint = _endMovePoint.WithY(_endMovePoint.Y + heightSegments * 32.0f);
		}

		bool IsWidthHandle(FPDragHandle handle)
		{
			switch (handle)
			{
				case FPDragHandle.TopLeft:
				case FPDragHandle.BottomLeft:
				case FPDragHandle.TopRight:
				case FPDragHandle.BottomRight:
				case FPDragHandle.MiddleLeft:
				case FPDragHandle.MiddleRight:
					return true;
				case FPDragHandle.MiddleTop:
				case FPDragHandle.MiddleBottom:
				case FPDragHandle.Center:
				case FPDragHandle.None:
				default:
					return false;
			}
		}

		bool IsHeightHandle(FPDragHandle handle)
		{
			switch (handle)
			{
				case FPDragHandle.TopLeft:
				case FPDragHandle.BottomLeft:
				case FPDragHandle.TopRight:
				case FPDragHandle.BottomRight:
				case FPDragHandle.MiddleTop:
				case FPDragHandle.MiddleBottom:
					return true;
				case FPDragHandle.MiddleLeft:
				case FPDragHandle.MiddleRight:
				case FPDragHandle.Center:
				case FPDragHandle.None:
				default:
					return false;
			}
		}

		Point PointFromHandleAroundRect(FPDragHandle handle, Rect rect)
		{
			switch (handle)
			{
				case FPDragHandle.TopLeft:
					return new Point(rect.X, rect.Y);
				case FPDragHandle.BottomLeft:
					return new Point(rect.X, rect.Y + rect.Height);
				case FPDragHandle.TopRight:
					return new Point(rect.X + rect.Width, rect.Y);
				case FPDragHandle.BottomRight:
					return new Point(rect.X + rect.Width, rect.Y + rect.Height);

				case FPDragHandle.MiddleLeft:
					return new Point(rect.X, rect.Y + rect.Height / 2.0f);
				case FPDragHandle.MiddleTop:
					return new Point(rect.X + rect.Width / 2.0f, rect.Y);
				case FPDragHandle.MiddleRight:
					return new Point(rect.X + rect.Width, rect.Y + rect.Height / 2.0f);
				case FPDragHandle.MiddleBottom:
					return new Point(rect.X + rect.Width / 2.0f, rect.Y + rect.Height);
				case FPDragHandle.Center:
					return new Point(rect.X + rect.Width / 2.0f, rect.Y + rect.Height / 2.0f);

				default:
					return new Point();
			}
		}

		public override void Render(DrawingContext context)
		{
			var bounds = ClippedBounds;
			context.FillRectangle(_backBrush, bounds);
			DrawGrid(context);

			FPDrawBuilder drawBuilder = new FPDrawBuilder(context, bounds);

			for (int i = 0; i < _gameObjects.Count; i++)
			{
				var gameObject = _gameObjects[i];
				if (gameObject.Draw(drawBuilder, bounds))
				{
					if (_selectedIndices.Contains(i))
					{
						DrawHandlesOnGameObject(context, gameObject);
					}
				}
			}

			drawBuilder.DrawAll();

			var draggedObject = this.DraggedObject;

			if (draggedObject != null)
				DrawHandlesOnGameObject(context, draggedObject);

			if (_drawingSelection)
			{
				context.FillRectangle(_selectionBrush, SelectionRect);
				context.DrawRectangle(_selectionPen, SelectionRect);
			}
		}
	}
}
