using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Globalization;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia;
using Avalonia.Media;

namespace IronJumpAvalonia.Game
{
    public class FPTrampoline : FPGameObject
    {
        static FPTextureArray _trampolineTextures = null;

        public static void InitTextures()
        {
            _trampolineTextures = new FPTextureArray();
			for (int i = 0; i < 3; i++)
			{
				_trampolineTextures.AddTexture(new FPTexture($"trampoline0{i + 1}.png"));
			}
        }

        int animationCounter;
        int textureIndex;
        int textureDirection;

        public float X { get; private set; }
        public float Y { get; private set; }
        public Rect Rect { get { return new Rect(X, Y, WidthSegments * 64.0f, 32.0f); } }
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
            Rect playerRect = player.Rect;

            if (!playerRect.Intersects(this.Rect))
            {
                playerRect = playerRect.WithHeight(playerRect.Height + FPPlayer.tolerance);
                if (playerRect.Intersects(this.Rect))
                    player.MoveY = 9.5f;
            }

            foreach (var gameObject in game.GameObjects)
            {
                if (gameObject.IsMovable)
                {
                    FPMovablePlatform movable = (FPMovablePlatform)gameObject;
                    Rect movableRect = movable.Rect;
                    movableRect = movableRect.WithHeight(movableRect.Height + FPPlayer.tolerance);
                    Rect intersection = movableRect.Intersect(this.Rect);
                    if (!intersection.IsEmptyWithTolerance() && intersection.Width > 30.0f)
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

        public void Draw(DrawingContext context)
        {
            var texture = _trampolineTextures[textureIndex];
            texture.Draw(context, X, Y, WidthSegments);
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
