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
    public class FPSpeedPowerUp : FPGameObject
    {
        static FPTexture speedPowerUpTexture;

        public static void InitTextures(FPCanvas canvas)
        {
            speedPowerUpTexture = canvas.CreateTexture(Resources.speed_symbol);
        }

        int speedUpCounter;

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

        public FPSpeedPowerUp()
        {
            X = 0.0f;
            Y = 0.0f;
            IsVisible = true;
            speedUpCounter = 0;
        }

        public void Move(float offsetX, float offsetY)
        {
            X += offsetX;
            Y += offsetY;
        }

        public FPGameObject Duplicate(float offsetX, float offsetY)
        {
            FPSpeedPowerUp duplicated = new FPSpeedPowerUp();
            duplicated.Move(X + offsetX, Y + offsetY);
            return duplicated;
        }

        public void Update(FPGameProtocol game)
        {
            FPPlayer player = (FPPlayer)game.Player;
            RectangleF playerRect = player.Rect;
            if (speedUpCounter > 0)
            {
                speedUpCounter++;
                if (speedUpCounter > FPPlayer.maxSpeedUpCount)
                {
                    speedUpCounter = 0;
                    IsVisible = true;
                }
            }
            else if (playerRect.IntersectsWith(this.Rect))
            {
                speedUpCounter = 1;
                player.SpeedUpCounter = 1;
                IsVisible = false;
            }
        }

        public void Draw(FPCanvas canvas)
        {
            speedPowerUpTexture.Draw(new PointF(X, Y));
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
