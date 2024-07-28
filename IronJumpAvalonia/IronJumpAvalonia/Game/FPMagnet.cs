using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace IronJumpAvalonia.Game
{
    public class FPMagnet : FPGameObject
    {
        static FPTexture _magnetTexture;

        public static void InitTextures()
        {
            _magnetTexture = new FPTexture("magnet.png");
		}

        public float X { get; private set; }
        public float Y { get; private set; }
        public Rect Rect { get { return new Rect(X, Y, WidthSegments * 32.0f, 32.0f); } }
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
            Rect playerRect = player.Rect;
            Rect selfRect = new Rect(X + 18.0, Y + 32.0, Rect.Width - 18.0 * 2, Rect.Height + 32.0 * 4);
            if (selfRect.Intersects(playerRect))
            {
                if (player.MoveY < 5.0f)
                    player.MoveY = FPMath.flerpf(player.MoveY, 5.0f, 0.3f);
            }
        }

        public void Draw(DrawingContext context)
        {
            _magnetTexture.Draw(context, X, Y, WidthSegments);
        }

        public void InitFromElement(XElement element)
        {
            X = element.ParseFloat("x");
            Y = element.ParseFloat("y");
            WidthSegments = element.ParseInt("widthSegments");
        }

        public void WriteToElement(XElement element)
        {
            element.WriteFloat("x", X);
            element.WriteFloat("y", Y);
            element.WriteInt("widthSegments", WidthSegments);
        }
    }
}
