using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using IronJumpAvalonia.Game;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronJumpAvalonia.Controls
{
	public class GamePlayer : Control
	{
		public FPGame Game { get; set; }
		HashSet<int> _pressedKeys = new HashSet<int>();

		protected override void OnLoaded(RoutedEventArgs e)
		{
			base.OnLoaded(e);
			this.Focusable = true;
			DispatcherTimer.Run(new Func<bool>(() =>
			{
				if (Game != null)
				{
					Vector inputAcceleration = new Vector();
					if (_pressedKeys.Contains((int)Key.Left))
						inputAcceleration = inputAcceleration.WithX(-1.0f);
					else if (_pressedKeys.Contains((int)Key.Right))
						inputAcceleration = inputAcceleration.WithX(1.0f);
					if (_pressedKeys.Contains((int)Key.Up))
						inputAcceleration = inputAcceleration.WithY(1.0f);

					Game.InputAcceleration = inputAcceleration;
					Game.Update();
					InvalidateVisual();
				}
				return true;
			}), TimeSpan.FromMilliseconds(16));
		}

		public override void Render(DrawingContext context)
		{
			Stopwatch sw = Stopwatch.StartNew();

			if (Game != null)
				Game.Draw(context);

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

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			_pressedKeys.Add(((int)e.Key));
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);
			_pressedKeys.Remove(((int)e.Key));
		}
	}
}
