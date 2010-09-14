using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GLCanvas;
using IronJumpLevelEditor_CS.Properties;

namespace IronJumpLevelEditor_CS.PortedClasses
{
    public class FPExit : FPGameObject
    {
        static FPTexture exitTexture = null;

        public static void InitTextures(FPCanvas canvas)
        {
            exitTexture = canvas.CreateTexture(Resources.exit);
        }

        public float X { get; private set; }
        public float Y { get; private set; }
        public RectangleF Rect { get { return new RectangleF(X, Y, 64.0f, 64.0f); } }
        public bool IsVisible { get; set; }
        public bool IsTransparent { get { return true; } }
        public bool IsPlatform { get { return false; } }
        public bool IsMovable { get { return false; } }
        public int WidthSegments { get { return 0; } set { } }
        public int HeightSegments { get { return 0; } set { } }
        public FPGameObject NextPart { get { return null; } }

        public FPExit()
        {
            X = 0.0f;
            Y = 0.0f;
            IsVisible = true;
        }

        public void Move(float offsetX, float offsetY)
        {
            X += offsetX;
            Y += offsetY;
        }

        public void Draw(FPCanvas canvas)
        {
            exitTexture.Draw(new PointF(X, Y));       
        }

        public FPGameObject Duplicate(float offsetX, float offsetY)
        {
            FPExit duplicated = new FPExit();
            duplicated.Move(X + offsetX, Y + offsetY);
            return duplicated;
        }

        public void Update(FPGameProtocol game)
        {
            if (!IsVisible)
                return;
    
            var playerRect = game.Player.Rect;
            if (playerRect.IntersectsWith(this.Rect))
               IsVisible = false;
        }
    }
}
