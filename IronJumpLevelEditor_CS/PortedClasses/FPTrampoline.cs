using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GLCanvas;
using IronJumpLevelEditor_CS.Properties;
using System.Xml.Linq;
using System.Globalization;

namespace IronJumpLevelEditor_CS.PortedClasses
{
    public class FPTrampoline : FPGameObject
    {
        static FPTextureArray trampolineTextures = null;

        public static void InitTextures(FPCanvas canvas)
        {
            trampolineTextures = new FPTextureArray();
            trampolineTextures.AddTexture(canvas, Resources.trampoline01);
            trampolineTextures.AddTexture(canvas, Resources.trampoline02);
            trampolineTextures.AddTexture(canvas, Resources.trampoline03);
        }

        int animationCounter;
        int textureIndex;
        int textureDirection;

        public float X { get; private set; }
        public float Y { get; private set; }
        public RectangleF Rect { get { return new RectangleF(X, Y, WidthSegments * 64.0f, 32.0f); } }
        public bool IsVisible { get; set; }
        public bool IsTransparent { get { return true; } }
        public bool IsPlatform { get { return true; } }
        public bool IsMovable { get { return false; } }
        public int WidthSegments { get; set; }
        public int HeightSegments { get; set; }
        public FPGameObject NextPart { get { return null; } }

        public FPTrampoline()
        {
            X = 0.0f;
            Y = 0.0f;
            WidthSegments = 1;
            IsVisible = true;
            textureIndex = 0;
            textureDirection = 1;
            animationCounter = 0;
        }

        public void Move(float offsetX, float offsetY)
        {
            X += offsetX;
            Y += offsetY;
        }

        public FPGameObject Duplicate(float offsetX, float offsetY)
        {
            FPTrampoline duplicated = new FPTrampoline();
            duplicated.WidthSegments = WidthSegments;
            duplicated.Move(X + offsetX, Y + offsetY);
            return duplicated;
        }

        public void Update(FPGameProtocol game)
        {
            FPPlayer player = (FPPlayer)game.Player;
            RectangleF playerRect = player.Rect;

            if (!playerRect.IntersectsWith(this.Rect))
            {
                playerRect.Height += FPPlayer.tolerance;
                if (playerRect.IntersectsWith(this.Rect))
                    player.MoveY = 9.5f;
            }

            foreach (var gameObject in game.GameObjects)
            {
                if (gameObject.IsMovable)
                {
                    FPMovablePlatform movable = (FPMovablePlatform)gameObject;
                    RectangleF movableRect = movable.Rect;
                    movableRect.Height += FPPlayer.tolerance;
                    RectangleF intersection = RectangleF.Intersect(movableRect, this.Rect);
                    if (!intersection.IsEmpty && intersection.Width > 30.0f)
                    {
                        movable.MoveY = 8.0f;
                    }
                }
            }

            if (++animationCounter > 5)
            {
                textureIndex += textureDirection;
                if (textureIndex < 0 || textureIndex >= 2)
                {
                    textureIndex -= textureDirection;
                    textureDirection = -textureDirection;
                }
                animationCounter = 0;
            }
        }

        public void Draw(FPCanvas canvas)
        {
            FPTexture texture = trampolineTextures[textureIndex];
            texture.Draw(new PointF(X, Y), WidthSegments, 1);
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
