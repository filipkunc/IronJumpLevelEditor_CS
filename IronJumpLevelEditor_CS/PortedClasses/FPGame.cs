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
        static Texture backgroundTexture = null;

        public static void InitTexture(Canvas canvas)
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

        public void Update()
        {
            diamondsPicked = 0;

            foreach (var gameObject in gameObjects)
            {
                if (!gameObject.IsVisible)
                {
                    //if (gameObject is FPDiamond)
                    //    diamondsPicked++;
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

        public void Draw(Canvas canvas)
        {
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
