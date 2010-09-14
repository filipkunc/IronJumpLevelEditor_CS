using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GLCanvas;
using IronJumpLevelEditor_CS.Properties;

namespace IronJumpLevelEditor_CS.PortedClasses
{
    public class FPGame : FPGameProtocol
    {
        static FPTexture backgroundTexture = null;

        public static void InitTexture(FPCanvas canvas)
        {
            backgroundTexture = canvas.CreateTexture(Resources.marbleblue);
        }

        List<FPGameObject> gameObjects;
        FPPlayer player;

        int diamondsPicked;
        int diamondsCount;

        PointF backgroundOffset;

        public PointF InputAcceleration { get; set; }

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

            InputAcceleration = PointF.Empty;
            Width = width;
            Height = height;
            backgroundOffset = PointF.Empty;

            diamondsPicked = 0;
            diamondsCount = 0;
        }

        public FPGame(float width, float height, IEnumerable<FPGameObject> levelObjects)
            : this(width, height)
        {
            PointF playerPosition = PointF.Empty;

            foreach (var gameObject in levelObjects)
            {
                if (gameObject is FPPlayer)
                {
                    playerPosition.X = gameObject.X;
                    playerPosition.Y = gameObject.Y;
                }
                else if (!(gameObject is FPElevatorEnd))
                {
                    if (gameObject is FPDiamond)
                        diamondsCount++;

                    gameObjects.Add(gameObject.Duplicate(0.0f, 0.0f));
                }
            }

            MoveWorld(player.X - playerPosition.X, player.Y - playerPosition.Y);
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

        public void Draw(FPCanvas canvas)
        {
            canvas.SetCurrentColor(Color.White);
            canvas.DisableBlend();

            PointF offset = new PointF(FPMath.fmodf(backgroundOffset.X, 32.0f) - 32.0f,
                                       FPMath.fmodf(backgroundOffset.Y, 32.0f) - 32.0f);

            backgroundTexture.Draw(offset, (int)(Width / backgroundTexture.Width) + 3, (int)(Height / backgroundTexture.Height) + 2);

            foreach (var gameObject in gameObjects)
            {
                if (!gameObject.IsVisible)
                    continue;

                if (!gameObject.IsTransparent)
                    gameObject.Draw(canvas);
            }

            canvas.EnableBlend();
            foreach (var gameObject in gameObjects)
            {
                if (!gameObject.IsVisible)
                    continue;

                if (gameObject.IsTransparent)
                    gameObject.Draw(canvas);
            }
            player.Draw(canvas);
            player.DrawSpeedUp(canvas);
        }

        public void MoveWorld(float x, float y)
        {
            foreach (var gameObject in gameObjects)
            {
                gameObject.Move(x, y);
            }

            backgroundOffset.X += x * 0.25f;
            backgroundOffset.Y += y * 0.25f;
        }
    }
}
