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

		//private void BeginResize(FPDragHandle handle)
		//{
		//	BeforeAction("Resize Selected");
		//}

		//void EndResize(PointF move)
		//{
		//	AfterAction("Resize Selected");
		//}

		//private void BeginMove()
		//{
		//	BeforeAction("Move Selected");
		//}

		//void EndMove(PointF move)
		//{
		//	AfterAction("Move Selected");
		//}

		private void DrawHandlesOnGameObject(DrawingContext context, FPGameObject gameObject)
		{

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
