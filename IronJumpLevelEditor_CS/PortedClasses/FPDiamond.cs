using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GLCanvas;
using IronJumpLevelEditor_CS.Properties;
using System.Xml.Linq;

namespace IronJumpLevelEditor_CS.PortedClasses
{
    public class FPDiamond : FPGameObject
    {
        static FPTexture diamondTexture = null;

        public static void InitTextures(FPCanvas canvas)
        {
            diamondTexture = canvas.CreateTexture(Resources.diamond);
        }

        public float X { get; private set; }
        public float Y { get; private set; }
        public RectangleF Rect { get { return new RectangleF(X, Y, 32.0f, 32.0f); } }
        public bool IsVisible { get; set; }
        public bool IsTransparent { get { return true; } }
        public bool IsPlatform { get { return false; } }
        public bool IsMovable { get { return false; } }
        public int WidthSegments { get { return 0; } set { } }
        public int HeightSegments { get { return 0; } set { } }
        public FPGameObject NextPart { get { return null; } }

        public FPDiamond()
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
            diamondTexture.Draw(new PointF(X, Y));       
        }

        public FPGameObject Duplicate(float offsetX, float offsetY)
        {
            FPDiamond duplicated = new FPDiamond();
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

        public void InitFromElement(XElement element)
        {
            X = element.ParseFloat("x");
            Y = element.ParseFloat("y");            
        }

        public void WriteToElement(XElement element)
        {
            element.WriteFloat("x", X);
            element.WriteFloat("y", Y);            
        }
    }
}
