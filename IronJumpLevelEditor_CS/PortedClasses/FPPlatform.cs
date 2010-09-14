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
    public class FPPlatform : FPGameObject
    {
        static FPTexture platformTexture = null;

        public static void InitTextures(FPCanvas canvas)
        {
            platformTexture = canvas.CreateTexture(Resources.plos_marble);
        }

        public float X { get; private set; }
        public float Y { get; private set; }
        public RectangleF Rect { get { return new RectangleF(X, Y, WidthSegments * 32.0f, HeightSegments * 32.0f); } }
        public bool IsVisible { get; set; }
        public bool IsTransparent { get { return false; } }
        public bool IsPlatform { get { return true; } }
        public bool IsMovable { get { return false; } }
        public int WidthSegments { get; set; }
        public int HeightSegments { get; set; }
        public FPGameObject NextPart { get { return null; } }

        public FPPlatform()
        {
            X = 0.0f;
            Y = 0.0f;
            WidthSegments = 1;
            HeightSegments = 1;
            IsVisible = true;
        }

        public void Move(float offsetX, float offsetY)
        {
            X += offsetX;
            Y += offsetY;
        }

        public void Draw(FPCanvas canvas)
        {
            platformTexture.Draw(new PointF(X, Y), WidthSegments, HeightSegments);
        }

        public FPGameObject Duplicate(float offsetX, float offsetY)
        {
            FPPlatform duplicated = new FPPlatform();
            duplicated.Move(X + offsetX, Y + offsetY);
            duplicated.WidthSegments = WidthSegments;
            duplicated.HeightSegments = HeightSegments;
            return duplicated;
        }

        public void Update(FPGameProtocol game)
        {
            
        }

        public void InitFromElement(XElement element)
        {
            X = element.ParseFloat("x");
            Y = element.ParseFloat("y");
            WidthSegments = element.ParseInt("widthSegments");
            HeightSegments = element.ParseInt("heightSegments");
        }

        public void WriteToElement(XElement element)
        {
            element.WriteFloat("x", X);
            element.WriteFloat("y", Y);
            element.WriteInt("widthSegments", WidthSegments);
            element.WriteInt("heightSegments", HeightSegments);
        }
    }
}
