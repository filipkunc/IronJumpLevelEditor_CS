using Avalonia.Controls;
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

		protected override void OnLoaded(RoutedEventArgs e)
		{
			base.OnLoaded(e);
			DispatcherTimer.Run(new Func<bool>(() =>
			{
				if (Game != null)
				{
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
	}
}
