using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Avalonia.Media.Imaging;
using Avalonia;
using Avalonia.Platform;
using Avalonia.Media;

namespace IronJumpAvalonia.Game
{
    public class FPGame : FPGameProtocol
    {
        static FPTexture _backgroundTexture = null;

        public static void InitTexture()
        {
            _backgroundTexture = new FPTexture("marbleblue.png");
		}

        List<FPGameObject> _levelObjects;
        List<FPGameObject> _gameObjects;
        FPPlayer _player;

        int _diamondsPicked;
        int _diamondsCount;

        Vector _backgroundOffset;

		public Vector InputAcceleration { get; set; }

        public float Width { get; private set; }
        public float Height { get; private set; }

        public int DiamondsPicked { get { return _diamondsPicked; } }
        public int DiamondsCount { get { return _diamondsCount; } }

        public FPGameObject Player
        {
            get { return _player; }
        }

        public List<FPGameObject> GameObjects
        {
            get { return _gameObjects; }
        }

        public FPGame(float width, float height)
        {
            Width = width;
            Height = height;
            Reset();
        }

        public FPGame(float width, float height, IEnumerable<FPGameObject> levelObjects)
            : this(width, height)
        {
            _levelObjects = levelObjects.ToList();
			Reset();
		}

        public void Reset()
        {
            _gameObjects = new List<FPGameObject>();
			_player = new FPPlayer(Width, Height);

            InputAcceleration = Vector.Zero;
            _backgroundOffset = Vector.Zero;

            _diamondsPicked = 0;
            _diamondsCount = 0;

            if (_levelObjects != null)
            {

                float playerPositionX = 0.0f;
                float playerPositionY = 0.0f;

                foreach (var gameObject in _levelObjects)
                {
                    if (gameObject is FPPlayer)
                    {
                        playerPositionX = gameObject.X;
                        playerPositionY = gameObject.Y;
                    }
                    else if (!(gameObject is FPElevatorEnd))
                    {
                        if (gameObject is FPDiamond)
                            _diamondsCount++;

                        _gameObjects.Add(gameObject.Duplicate(0.0f, 0.0f));
                    }
                }

                MoveWorld(_player.X - playerPositionX, _player.Y - playerPositionY);
            }
        }

        public void Resize(float width, float height)
        {
            if (Width != width || Height != height)
            {
                Width = width;
                Height = height;
                float oldPlayerX = _player.X;
                float oldPlayerY = _player.Y;
                float newPlayerX = width / 2.0f - 32.0f / 2.0f;
                float newPlayerY = height / 2.0f - 32.0f / 2.0f;
                float offsetX = newPlayerX - oldPlayerX;
                float offsetY = newPlayerY - oldPlayerY;
                _player.Move(offsetX, offsetY);
                MoveWorld(offsetX, offsetY);
            }
        }
        public void Update()
        {
            _diamondsPicked = 0;

            foreach (var gameObject in _gameObjects)
            {
                if (!gameObject.IsVisible)
                {
                    if (gameObject is FPDiamond)
                        _diamondsPicked++;
                }

                if (!gameObject.IsMovable)
                    gameObject.Update(this);
            }

            foreach (var gameObject in _gameObjects)
            {
                if (gameObject.IsMovable)
                    gameObject.Update(this);
            }

            _player.Update(this);
        }

        public void Draw(DrawingContext context)
        {
            var bounds = new Rect(0, 0, Width, Height);
            var drawBuilder = new FPDrawBuilder(context, bounds);

            Vector offset = new Vector(FPMath.fmodf((float)_backgroundOffset.X, 32.0f) - 32.0f,
                                       FPMath.fmodf((float)_backgroundOffset.Y, 32.0f) - 32.0f);

            drawBuilder.AddSprite(_backgroundTexture,
                (float)offset.X, (float)offset.Y,
                (int)(Width / _backgroundTexture.Size.Width) + 3,
                (int)(Height / _backgroundTexture.Size.Height) + 3);

            foreach (var gameObject in _gameObjects)
            {
                if (!gameObject.IsVisible)
                    continue;

                if (!gameObject.IsTransparent)
                    gameObject.Draw(drawBuilder, bounds);
            }

            foreach (var gameObject in _gameObjects)
            {
                if (!gameObject.IsVisible)
                    continue;

                if (gameObject.IsTransparent)
                    gameObject.Draw(drawBuilder, bounds);
            }
            
            _player.Draw(drawBuilder, bounds);
            _player.DrawSpeedUp(drawBuilder, bounds);

            drawBuilder.DrawAll();

			FormattedText diamondsText = new FormattedText(
				$"Diamonds: {_diamondsPicked}/{_diamondsCount}",
				CultureInfo.GetCultureInfo("en-us"),
				FlowDirection.LeftToRight,
				new Typeface("Verdana"),
				16,
				Brushes.White);

			context.DrawText(diamondsText, new Point(2, 18));

            if (_player.SpeedUpCounter > 0)
            {
                FormattedText speedUpText = new FormattedText(
					string.Format(CultureInfo.InvariantCulture, "Speed: {0:f1}s", (FPPlayer.maxSpeedUpCount - _player.SpeedUpCounter) / 60.0f),
                    CultureInfo.GetCultureInfo("en-us"),
                    FlowDirection.LeftToRight,
                    new Typeface("Verdana"),
                    16,
                    new SolidColorBrush(Color.FromArgb(204, 127, 255, 255)));


                context.DrawText(speedUpText, new Point(2, bounds.Bottom - 18));
            }
		}

        public void MoveWorld(float x, float y)
        {
            foreach (var gameObject in _gameObjects)
            {
                gameObject.Move(x, y);
            }

            _backgroundOffset += new Vector(x * 0.25, y * 0.25);
        }
    }
}
