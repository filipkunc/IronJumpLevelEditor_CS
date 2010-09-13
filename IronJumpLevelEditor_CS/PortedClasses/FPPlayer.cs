using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GLCanvas;
using IronJumpLevelEditor_CS.Properties;
using System.Diagnostics;

namespace IronJumpLevelEditor_CS.PortedClasses
{
    public class FPPlayer : FPGameObject
    {
        static Texture playerTexture = null;

        public static void InitTextures(Canvas canvas)
        {
            playerTexture = canvas.CreateTexture(Resources.ball);
        }

        public const float tolerance = 3.0f;
        public const float maxSpeed = 5.8f;
        public const float speedPowerUp = 1.5f;
        public const float upSpeed = 7.0f;
        public const float maxFallSpeed = -15.0f;
        public const float acceleration = 1.1f;
        public const float deceleration = 1.1f * 0.2f;
        public const float changeDirectionSpeed = 3.0f;
        public const int maxSpeedUpCount = 60 * 6; // 60 FPS * 6 sec

        bool jumping;

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
        public float MoveX { get; set; }
        public float MoveY { get; set; }
        public float Alpha { get; set; }
        public float Rotation { get; set; }

        public FPPlayer()
        {
            X = 0.0f;
            Y = 0.0f;
            MoveX = 0.0f;
            MoveY = 0.0f;
            jumping = false;
            Alpha = 1.0f;
            IsVisible = true;
            Rotation = 0.0f;
        }

        public FPPlayer(float width, float height)
            : this()
        {
            X = width / 2.0f - 32.0f / 2.0f;
            Y = height / 2.0f - 32.0f / 2.0f;
        }

        public void Move(float offsetX, float offsetY)
        {
            X += offsetX;
            Y += offsetY;
        }

        public void Draw(Canvas canvas)
        {
            playerTexture.Draw(new PointF(X, Y), Rotation);            
        }

        public FPGameObject Duplicate(float offsetX, float offsetY)
        {
            FPPlayer duplicated = new FPPlayer();
            duplicated.Move(X + offsetX, Y + offsetY);
            return duplicated;
        }
        
        public void Update(FPGameProtocol game)
        {
            PointF inputAcceleration = game.InputAcceleration;
            bool moveLeftOrRight = false;

            if (speedUpCounter > 0)
            {
                if (++speedUpCounter > maxSpeedUpCount)
                {
                    speedUpCounter = 0;
                }
            }

            float currentMaxSpeed = speedUpCounter > 0 ? maxSpeed * speedPowerUp : maxSpeed;

            if (inputAcceleration.X < 0.0f)
            {
                if (MoveX < 0.0f)
                    MoveX += Math.Abs(inputAcceleration.X) * acceleration * changeDirectionSpeed;
                MoveX += Math.Abs(inputAcceleration.X) * acceleration;
                if (MoveX > currentMaxSpeed)
                    MoveX = currentMaxSpeed;

                moveLeftOrRight = true;
            }
            else if (inputAcceleration.X > 0.0f)
            {
                if (MoveX > 0.0f)
                    MoveX -= Math.Abs(inputAcceleration.X) * acceleration * changeDirectionSpeed;
                MoveX -= Math.Abs(inputAcceleration.X) * acceleration;
                if (MoveX < -currentMaxSpeed)
                    MoveX = -currentMaxSpeed;

                moveLeftOrRight = true;
            }

            if (!jumping && inputAcceleration.Y > 0.0f)
            {
                if (MoveY < upSpeed)
                    MoveY = upSpeed;
                jumping = true;
            }

            if (!moveLeftOrRight)
            {
                if (Math.Abs(MoveX) < deceleration)
                    MoveX = 0.0f;
                else if (MoveX > 0.0f)
                    MoveX -= deceleration;
                else if (MoveX < 0.0f)
                    MoveX += deceleration;
            }

            MoveY -= deceleration;
            if (MoveY < maxFallSpeed)
                MoveY = maxFallSpeed;
            jumping = true;

            game.MoveWorld(MoveX, 0.0f);
            if (CollisionLeftRight(game))
                MoveX = 0.0f;
            game.MoveWorld(0.0f, MoveY);
            CollisionUpDown(game);
            Rotation -= MoveX * 3.0f;

            Alpha += 0.07f;
            if (Alpha > (float)Math.PI)
                Alpha -= (float)Math.PI;
        }

        public bool CollisionLeftRight(FPGameProtocol game)
        {
            bool isColliding = false;

            foreach (var platform in game.GameObjects)
            {
                if (platform.IsPlatform)
                {
                    RectangleF intersection = RectangleF.Intersect(platform.Rect, this.Rect);
                    if (intersection.IsEmptyWithTolerance())
                        continue;

                    if (platform.Rect.Left > this.Rect.Left)
                    {
                        if (platform.IsMovable)
                        {
                            platform.Move(intersection.Width, 0.0f);
                            if (platform.CollisionLeftRight(game))
                            {
                                platform.Move(-intersection.Width, 0.0f);
                                game.MoveWorld(intersection.Width, 0.0f);
                                isColliding = true;
                            }
                        }
                        else
                        {
                            game.MoveWorld(intersection.Width, 0.0f);
                            isColliding = true;
                        }
                    }
                    else if (platform.Rect.Right < this.Rect.Right)
                    {
                        if (platform.IsMovable)
                        {
                            platform.Move(-intersection.Width, 0.0f);
                            if (platform.CollisionLeftRight(game))
                            {
                                platform.Move(intersection.Width, 0.0f);
                                game.MoveWorld(-intersection.Width, 0.0f);
                                isColliding = true;
                            }
                        }
                        else
                        {
                            game.MoveWorld(-intersection.Width, 0.0f);
                            isColliding = true;
                        }
                    }
                }
            }

            return isColliding;
        }

        public bool CollisionUpDown(FPGameProtocol game)
        {
            bool isColliding = false;

            foreach (var platform in game.GameObjects)
            {
                if (platform.IsPlatform)
                {
                    RectangleF intersection = RectangleF.Intersect(platform.Rect, this.Rect);
                    if (intersection.IsEmptyWithTolerance())
                        continue;

                    if (platform.Rect.Bottom < this.Rect.Bottom)
                    {
                        if (MoveY > 0.0f)
                            MoveY = 0.0f;

                        game.MoveWorld(0.0f, -intersection.Height);
                        isColliding = true;
                    }
                    else if (MoveY < 0.0f)
                    {
                        if (platform.Rect.Top > this.Rect.Bottom - tolerance + MoveY)
                        {
                            MoveY = 0.0f;
                            jumping = false;
                            game.MoveWorld(0.0f, intersection.Height);
                            isColliding = true;
                        }
                    }
                    else if (platform.Rect.Top > this.Rect.Bottom - tolerance + MoveY)
                    {
                        jumping = false;
                        game.MoveWorld(0.0f, intersection.Height);
                        isColliding = true;
                    }
                }
            }

            return isColliding;
        }
    }
}
