using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GLCanvas;
using IronJumpLevelEditor_CS.Properties;

namespace IronJumpLevelEditor_CS.PortedClasses
{
    public class FPMagnet : FPGameObject
    {
        static FPTexture magnetTexture;

        public static void InitTextures(FPCanvas canvas)
        {
            magnetTexture = canvas.CreateTexture(Resources.magnet);
        }

        public float X { get; private set; }
        public float Y { get; private set; }
        public RectangleF Rect { get { return new RectangleF(X, Y, WidthSegments * 32.0f, 32.0f); } }
        public bool IsVisible { get; set; }
        public bool IsTransparent { get { return true; } }
        public bool IsPlatform { get { return true; } }
        public bool IsMovable { get { return false; } }
        public int WidthSegments { get; set; }
        public int HeightSegments { get { return 0; } set { } }
        public FPGameObject NextPart { get { return null; } }

        public FPMagnet()
        {
            X = 0.0f;
            Y = 0.0f;
            WidthSegments = 1;
            IsVisible = true;
        }

        public void Move(float offsetX, float offsetY)
        {
            X += offsetX;
            Y += offsetY;
        }

        public FPGameObject Duplicate(float offsetX, float offsetY)
        {
            FPMagnet duplicated = new FPMagnet();
            duplicated.WidthSegments = WidthSegments;
            duplicated.Move(X + offsetX, Y + offsetY);
            return duplicated;
        }

        public void Update(FPGameProtocol game)
        {
            FPPlayer player = (FPPlayer)game.Player;
            RectangleF playerRect = player.Rect;
            RectangleF selfRect = this.Rect;
            selfRect.Y += 32.0f;
            selfRect.Height += 32.0f * 4;
            selfRect.X += 18.0f;
            selfRect.Width -= 18.0f * 2.0f;
            if (selfRect.IntersectsWith(playerRect))
            {
                if (player.MoveY < 5.0f)
                    player.MoveY = FPMath.flerpf(player.MoveY, 5.0f, 0.3f);
            }
        }

        public void Draw(FPCanvas canvas)
        {
            magnetTexture.Draw(new PointF(X, Y), WidthSegments, 1);
        }
    }
}
