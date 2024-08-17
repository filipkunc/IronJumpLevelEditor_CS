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
    public class FPMovablePlatform : FPGameObject
    {
        static FPTexture _movableTexture = null;

        public static void InitTextures()
        {
            _movableTexture = new FPTexture("movable.png");
		}

        public float X { get; private set; }
        public float Y { get; private set; }
        public Rect Rect { get { return new Rect(X, Y, WidthSegments * 32.0f, HeightSegments * 32.0f); } }
        public bool IsVisible { get; set; }
        public bool IsTransparent { get { return false; } }
        public bool IsPlatform { get { return true; } }
        public bool IsMovable { get { return true; } }
        public int WidthSegments { get; set; }
        public int HeightSegments { get; set; }
        public FPGameObject NextPart { get { return null; } }
        public float MoveY { get; set; }

        public FPMovablePlatform()
        {
            X = 0.0f;
            Y = 0.0f;
            WidthSegments = 1;
            HeightSegments = 1;
            IsVisible = true;
            MoveY = 0.0f;
        }

        public void Move(float offsetX, float offsetY)
        {
            X += offsetX;
            Y += offsetY;
        }

        public FPGameObject Duplicate(float offsetX, float offsetY)
        {
            FPMovablePlatform duplicated = new FPMovablePlatform();
            duplicated.WidthSegments = WidthSegments;
            duplicated.HeightSegments = HeightSegments;
            duplicated.Move(X + offsetX, Y + offsetY);
            return duplicated;
        }

        public void Update(FPGameProtocol game)
        {
            FPPlayer player = (FPPlayer)game.Player;
            Rect playerRect = player.Rect;

            MoveY -= FPPlayer.deceleration;
            if (MoveY < FPPlayer.maxFallSpeed)
                MoveY = FPPlayer.maxFallSpeed;

            Rect moveRect = this.Rect.WithMove(0.0f, -MoveY);

            if (playerRect.IntersectsWithTolerance(moveRect))
                MoveY = 0.0f;

            Y -= MoveY;
            CollisionUpDown(game);
        }

        public bool Draw(DrawingContext context, Rect bounds)
        {
            if (Rect.Intersects(bounds))
            {
                _movableTexture.Draw(context, X, Y, WidthSegments, HeightSegments);
                return true;
            }
            return false;
        }

        public bool CollisionLeftRight(FPGameProtocol game)
        {
            foreach (var platform in game.GameObjects)
            {
                if (platform != this && platform.IsPlatform)
                {
                    Rect intersection = platform.Rect.Intersect(this.Rect);
                    if (intersection.IsEmptyWithTolerance())
                        continue;

                    if (platform.Rect.Left > this.Rect.Left)
                        return true;
                    if (platform.Rect.Right < this.Rect.Right)
                        return true;
                }
            }

            return false;
        }

        public bool CollisionUpDown(FPGameProtocol game)
        {
            bool isColliding = false;

            foreach (var platform in game.GameObjects)
            {
                if (platform != this && platform.IsPlatform)
                {
                    Rect intersection = platform.Rect.Intersect(this.Rect);
                    if (intersection.IsEmptyWithTolerance())
                        continue;

                    if (platform.Rect.Bottom < this.Rect.Bottom)
                    {
                        if (MoveY > 0.0f)
                            MoveY = 0.0f;

                        Y += (float)intersection.Height;
                        isColliding = true;
                    }
                    else if (MoveY < 0.0f)
                    {
                        if (platform.Rect.Top > this.Rect.Bottom - FPPlayer.tolerance + MoveY)
                        {
                            MoveY = 0.0f;
                            Y -= (float)intersection.Height;
                            isColliding = true;
                        }
                    }
                    else if (platform.Rect.Top > this.Rect.Bottom - FPPlayer.tolerance + MoveY)
                    {
                        Y -= (float)intersection.Height;
                        isColliding = true;
                    }
                }
            }

            return isColliding;
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
