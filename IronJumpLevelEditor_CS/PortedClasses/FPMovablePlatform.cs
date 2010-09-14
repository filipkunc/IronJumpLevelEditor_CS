using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GLCanvas;
using IronJumpLevelEditor_CS.Properties;

namespace IronJumpLevelEditor_CS.PortedClasses
{
    public class FPMovablePlatform : FPGameObject
    {
        static FPTexture movableTexture = null;

        public static void InitTextures(FPCanvas canvas)
        {
            movableTexture = canvas.CreateTexture(Resources.movable);
        }

        public float X { get; private set; }
        public float Y { get; private set; }
        public RectangleF Rect { get { return new RectangleF(X, Y, WidthSegments * 32.0f, HeightSegments * 32.0f); } }
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
            RectangleF playerRect = player.Rect;

            MoveY -= FPPlayer.deceleration;
            if (MoveY < FPPlayer.maxFallSpeed)
                MoveY = FPPlayer.maxFallSpeed;

            RectangleF moveRect = this.Rect.WithMove(0.0f, -MoveY);

            if (playerRect.IntersectsWithTolerance(moveRect))
                MoveY = 0.0f;

            Y -= MoveY;
            CollisionUpDown(game);
        }

        public void Draw(FPCanvas canvas)
        {
            movableTexture.Draw(new PointF(X, Y), WidthSegments, HeightSegments);
        }

        public bool CollisionLeftRight(FPGameProtocol game)
        {
            foreach (var platform in game.GameObjects)
            {
                if (platform != this && platform.IsPlatform)
                {
                    RectangleF intersection = RectangleF.Intersect(platform.Rect, this.Rect);
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
                    RectangleF intersection = RectangleF.Intersect(platform.Rect, this.Rect);
                    if (intersection.IsEmptyWithTolerance())
                        continue;

                    if (platform.Rect.Bottom < this.Rect.Bottom)
                    {
                        if (MoveY > 0.0f)
                            MoveY = 0.0f;

                        Y += intersection.Height;
                        isColliding = true;
                    }
                    else if (MoveY < 0.0f)
                    {
                        if (platform.Rect.Top > this.Rect.Bottom - FPPlayer.tolerance + MoveY)
                        {
                            MoveY = 0.0f;
                            Y -= intersection.Height;
                            isColliding = true;
                        }
                    }
                    else if (platform.Rect.Top > this.Rect.Bottom - FPPlayer.tolerance + MoveY)
                    {
                        Y -= intersection.Height;
                        isColliding = true;
                    }
                }
            }

            return isColliding;
        }
    }
}
