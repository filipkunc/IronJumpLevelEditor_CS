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

namespace IronJumpAvalonia.Controls
{
	public class LevelEditor : Control
	{
		Brush _backBrush = new SolidColorBrush(Color.FromRgb(55, 60, 89));
		Pen _gridPen = new Pen(new SolidColorBrush(Color.FromArgb((int)(0.2 * 255), 255, 255, 255)));
		Brush _selectionBrush = new SolidColorBrush(Color.FromArgb(50, 255, 255, 255));
		Pen _selectionPen = new Pen(new SolidColorBrush(Color.FromArgb(230, 255, 255, 255)));

		Rect ClippedBounds => new Rect(-Bounds.Position, this.GetTransformedBounds()!.Value.Clip.Size);

		Bitmap _ballImage = new Bitmap(AssetLoader.Open(new Uri("avares://IronJumpAvalonia/Assets/ball.png")));


		bool _drawingSelection = false;
		Point _beginSelection;
		Point _endSelection;

		Rect SelectionRect => new Rect(
			new Point(Math.Min(_beginSelection.X, _endSelection.X) + 0.5, Math.Min(_beginSelection.Y, _endSelection.Y) + 0.5),
			new Point(Math.Max(_beginSelection.X, _endSelection.X) + 0.5, Math.Max(_beginSelection.Y, _endSelection.Y) + 0.5)
		);

		void DrawGrid(DrawingContext context)
		{
			var rect = ClippedBounds;

			int offsetX = (int)rect.X % 32;
			int offsetY = (int)rect.Y % 32;

			for (int y = (int)rect.Top + offsetY; y < (int)rect.Bottom + 32; y += 32)
				context.DrawLine(_gridPen, new Point(rect.Left, y + 0.5), new Point(rect.Right + 32, y + 0.5));

			for (int x = (int)rect.Left + offsetX; x < (int)rect.Right + 32; x += 32)
				context.DrawLine(_gridPen, new Point(x + 0.5, rect.Top), new Point(x + 0.5, rect.Bottom + 32));
		}

		protected override void OnLoaded(RoutedEventArgs e)
		{
			base.OnLoaded(e);
			Parent!.PropertyChanged += Parent_PropertyChanged;
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

		public override void Render(DrawingContext context)
		{
			context.FillRectangle(_backBrush, ClippedBounds);
			DrawGrid(context);

			// Create the initial formatted text string.
			FormattedText formattedText = new FormattedText(
				$"ClippedBounds: {ClippedBounds}",
				CultureInfo.GetCultureInfo("en-us"),
				FlowDirection.LeftToRight,
				new Typeface("Verdana"),
				32,
				Brushes.White);

			context.DrawText(formattedText, -Bounds.Position);

			context.DrawImage(_ballImage, new Rect(50, 50, 32, 32));

			if (_drawingSelection)
			{
				context.FillRectangle(_selectionBrush, SelectionRect);
				context.DrawRectangle(_selectionPen, SelectionRect);
			}
		}
	}
}
