using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace IronJumpAvalonia.Game
{
    public class FPDiamond : FPGameObject
    {
        static FPTexture _diamondTexture = null;

        public static void InitTextures()
        {
            _diamondTexture = new FPTexture("diamond.png");
        }

        public float X { get; private set; }
        public float Y { get; private set; }
        public Rect Rect { get { return new Rect(X, Y, 32.0f, 32.0f); } }
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

        public bool Draw(FPDrawBuilder drawBuilder, Rect bounds)
        {
            if (Rect.Intersects(bounds))
            {
                drawBuilder.AddSprite(_diamondTexture, X, Y);
                return true;
            }
            return false;
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
            if (playerRect.Intersects(this.Rect))
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
