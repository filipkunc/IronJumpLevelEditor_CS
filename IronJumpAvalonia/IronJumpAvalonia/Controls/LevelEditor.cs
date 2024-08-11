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
			//SelectedFactory = null;

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
			_drawingSelection = true;
			_beginSelection = _endSelection = e.GetPosition(this);
			InvalidateVisual();
		}

		protected override void OnPointerMoved(PointerEventArgs e)
		{
			base.OnPointerMoved(e);
			if (_drawingSelection)
			{
				_endSelection = e.GetPosition(this);
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
				InvalidateVisual();
			}
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
			Stopwatch sw = Stopwatch.StartNew();

			var bounds = ClippedBounds;
			context.FillRectangle(_backBrush, bounds);
			DrawGrid(context);

			for (int i = 0; i < _gameObjects.Count; i++)
			{
				var gameObject = _gameObjects[i];
				if (gameObject.Rect.Intersects(bounds))
				{
					gameObject.Draw(context);
					if (_selectedIndices.Contains(i))
					{
						DrawHandlesOnGameObject(context, gameObject);
					}
				}
			}

			var draggedObject = this.DraggedObject;

			if (draggedObject != null)
				DrawHandlesOnGameObject(context, draggedObject);

			if (_drawingSelection)
			{
				context.FillRectangle(_selectionBrush, SelectionRect);
				context.DrawRectangle(_selectionPen, SelectionRect);
			}

			sw.Stop();

			FormattedText formattedText = new FormattedText(
				$"Frame time: {sw.ElapsedMilliseconds}ms",
				CultureInfo.GetCultureInfo("en-us"),
				FlowDirection.LeftToRight,
				new Typeface("Verdana"),
				16,
				Brushes.White);

			context.DrawText(formattedText, -Bounds.Position);
		}
	}
}
