using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Threading;
using IronJumpAvalonia.Game;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Essentials;

namespace IronJumpAvalonia.Controls
{
	public class GamePlayer : Control
	{
		public FPGame Game { get; set; }
		HashSet<int> _pressedKeys = new HashSet<int>();
		float _lastAcceleration = 0.0f;
		TopLevel _topLevel;
		TimeSpan _lastTime = TimeSpan.Zero;

		protected override void OnLoaded(RoutedEventArgs e)
		{
			base.OnLoaded(e);
			_topLevel = TopLevel.GetTopLevel(this);
			_topLevel.RendererDiagnostics.DebugOverlays = RendererDebugOverlays.Fps;

			try
			{
				Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
				Accelerometer.Start(SensorSpeed.Fastest);
			}
			catch { }

			this.Focusable = true;
			this.FocusAdorner = null;
			this.Focus();
			AnimationUpdate(TimeSpan.Zero);
		}

		void AnimationUpdate(TimeSpan timeSpan)
		{
			if (Game != null)
			{
				Game.Resize((float)_topLevel.Width, (float)_topLevel.Height);
				var frameTime = TimeSpan.FromMilliseconds(16);
				if (timeSpan - _lastTime > frameTime)
				{
					Game.Update();
					_lastTime = timeSpan;
				}
				ResetIfGameOver();
				InvalidateVisual();
			}
			_topLevel.RequestAnimationFrame(AnimationUpdate);
		}

		private void Accelerometer_ReadingChanged(object? sender, AccelerometerChangedEventArgs e)
		{
			var acceleration = e.Reading.Acceleration;
			var inputAcceleration = new Vector(acceleration.Y, -acceleration.X);
			if (Math.Abs(inputAcceleration.X) < 0.02f)
				inputAcceleration = inputAcceleration.WithX(0.0f);

			if (inputAcceleration.Y + 0.2f > 0.1f)
				inputAcceleration = inputAcceleration.WithY(1.0f);

			Game.InputAcceleration = inputAcceleration;
		}

		void UpdateInputAccelerationViaKeyboard()
		{
			Vector inputAcceleration = new Vector();
			if (_pressedKeys.Contains((int)Key.Left))
				inputAcceleration = inputAcceleration.WithX(-1.0f);
			else if (_pressedKeys.Contains((int)Key.Right))
				inputAcceleration = inputAcceleration.WithX(1.0f);
			if (_pressedKeys.Contains((int)Key.Up))
				inputAcceleration = inputAcceleration.WithY(1.0f);

			Game.InputAcceleration = inputAcceleration;
		}
		
		void ResetIfGameOver()
		{
			if (Game == null)
				return;

			var playerY = Game.Player.Rect.Bottom;
			foreach (var gameObject in Game.GameObjects)
			{
				if (gameObject.IsPlatform && !gameObject.IsMovable)
				{
					var gameObjectY = gameObject.Rect.Bottom;
					if (playerY < gameObjectY)
					{
						return;
					}
				}
			}

			Game.Reset();
		}

		public override void Render(DrawingContext context)
		{
			if (Game != null)
				Game.Draw(context);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			_pressedKeys.Add(((int)e.Key));
			UpdateInputAccelerationViaKeyboard();
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);
			_pressedKeys.Remove(((int)e.Key));
			UpdateInputAccelerationViaKeyboard();
		}
	}
}
