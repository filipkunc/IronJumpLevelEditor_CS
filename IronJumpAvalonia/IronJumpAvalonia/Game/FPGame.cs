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

        List<FPGameObject> gameObjects;
        FPPlayer player;

        int diamondsPicked;
        int diamondsCount;

        Vector _backgroundOffset;

        public Vector InputAcceleration { get; set; }

        public float Width { get; private set; }
        public float Height { get; private set; }

        public int DiamondsPicked { get { return diamondsPicked; } }
        public int DiamondsCount { get { return diamondsCount; } }

        public FPGameObject Player
        {
            get { return player; }
        }

        public List<FPGameObject> GameObjects
        {
            get { return gameObjects; }
        }

        public FPGame(float width, float height)
        {
            gameObjects = new List<FPGameObject>();
            player = new FPPlayer(width, height);

            InputAcceleration = Vector.Zero;
            Width = width;
            Height = height;
            _backgroundOffset = Vector.Zero;

            diamondsPicked = 0;
            diamondsCount = 0;
        }

        public FPGame(float width, float height, IEnumerable<FPGameObject> levelObjects)
            : this(width, height)
        {
            float playerPositionX = 0.0f;
            float playerPositionY = 0.0f;

            foreach (var gameObject in levelObjects)
            {
                if (gameObject is FPPlayer)
                {
                    playerPositionX = gameObject.X;
                    playerPositionY = gameObject.Y;
                }
                else if (!(gameObject is FPElevatorEnd))
                {
                    if (gameObject is FPDiamond)
                        diamondsCount++;

                    gameObjects.Add(gameObject.Duplicate(0.0f, 0.0f));
                }
            }

            MoveWorld(player.X - playerPositionX, player.Y - playerPositionY);
        }

        public void Update()
        {
            diamondsPicked = 0;

            foreach (var gameObject in gameObjects)
            {
                if (!gameObject.IsVisible)
                {
                    if (gameObject is FPDiamond)
                        diamondsPicked++;
                }

                if (!gameObject.IsMovable)
                    gameObject.Update(this);
            }

            foreach (var gameObject in gameObjects)
            {
                if (gameObject.IsMovable)
                    gameObject.Update(this);
            }

            player.Update(this);
        }

        public void Draw(DrawingContext context)
        {
            Vector offset = new Vector(FPMath.fmodf((float)_backgroundOffset.X, 32.0f) - 32.0f,
                                       FPMath.fmodf((float)_backgroundOffset.Y, 32.0f) - 32.0f);

            _backgroundTexture.Draw(context, (float)offset.X, (float)offset.Y, (int)(Width / _backgroundTexture.Size.Width) + 3, (int)(Height / _backgroundTexture.Size.Height) + 2);

            foreach (var gameObject in gameObjects)
            {
                if (!gameObject.IsVisible)
                    continue;

                if (!gameObject.IsTransparent)
                    gameObject.Draw(context);
            }

            foreach (var gameObject in gameObjects)
            {
                if (!gameObject.IsVisible)
                    continue;

                if (gameObject.IsTransparent)
                    gameObject.Draw(context);
            }
            player.Draw(context);
            player.DrawSpeedUp(context);

            //canvas.SetCurrentColor(Color.FromArgb(204, Color.White));

            //fontTexture.DrawText(string.Format("Diamonds: {0}/{1}", diamondsPicked, diamondsCount), new PointF(3.0f, 0.0f), 16, 16, 32, 13);

            //string speedUpText = null;
            //if (player.SpeedUpCounter > 0)
            //    speedUpText = string.Format(CultureInfo.InvariantCulture, "{0:f1}", (FPPlayer.maxSpeedUpCount - player.SpeedUpCounter) / 60.0f);

            //if (!string.IsNullOrEmpty(speedUpText))
            //{
            //    canvas.SetCurrentColor(Color.FromArgb(204, 127, 255, 255));
            //    fontTexture.DrawText(speedUpText, new PointF(430.0f, 285.0f), 16, 16, 32, 13);
            //}
        }

        public void MoveWorld(float x, float y)
        {
            foreach (var gameObject in gameObjects)
            {
                gameObject.Move(x, y);
            }

            _backgroundOffset += new Vector(x * 0.25, y * 0.25);
        }
    }
}
