using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml.Linq;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia;
using Avalonia.Platform;

namespace IronJumpAvalonia.Game
{
    public class FPPlayer : FPGameObject
    {
        static FPTexture playerTexture = null;
        static FPTexture jumpTexture = null;

        public static void InitTextures()
        {
			
			playerTexture = new FPTexture("ball.png");
			jumpTexture = new FPTexture("speed.png");
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
        public float MoveX { get; set; }
        public float MoveY { get; set; }
        public float Alpha { get; set; }
        public float Rotation { get; set; }
        public int SpeedUpCounter { get; set; }

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

        public void Draw(DrawingContext context)
        {
            playerTexture.Draw(context, X, Y, Rotation);
        }

        public void DrawSpeedUp(DrawingContext context)
        {
            //if (SpeedUpCounter <= 0)
            //    return;

            //float value = Math.Abs(FPMath.sinf(Alpha)) * 0.5f + 0.5f;
            //canvas.SetCurrentColor(Color.FromArgb((int)(255 * value), Color.White));

            //PointF position = new PointF(X - 16.0f, Y - 16.0f);
            //jumpTexture.Draw(position);
        }

        public FPGameObject Duplicate(float offsetX, float offsetY)
        {
            FPPlayer duplicated = new FPPlayer();
            duplicated.Move(X + offsetX, Y + offsetY);
            return duplicated;
        }

        public void Update(FPGameProtocol game)
        {
            var inputAcceleration = game.InputAcceleration;
            bool moveLeftOrRight = false;

            if (SpeedUpCounter > 0)
            {
                if (++SpeedUpCounter > maxSpeedUpCount)
                {
                    SpeedUpCounter = 0;
                }
            }

            float currentMaxSpeed = SpeedUpCounter > 0 ? maxSpeed * speedPowerUp : maxSpeed;

            if (inputAcceleration.X < 0.0f)
            {
                if (MoveX < 0.0f)
                    MoveX += (float)Math.Abs(inputAcceleration.X) * acceleration * changeDirectionSpeed;
                MoveX += (float)Math.Abs(inputAcceleration.X) * acceleration;
                if (MoveX > currentMaxSpeed)
                    MoveX = currentMaxSpeed;

                moveLeftOrRight = true;
            }
            else if (inputAcceleration.X > 0.0f)
            {
                if (MoveX > 0.0f)
                    MoveX -= (float)Math.Abs(inputAcceleration.X) * acceleration * changeDirectionSpeed;
                MoveX -= (float)Math.Abs(inputAcceleration.X) * acceleration;
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
                    Rect intersection = platform.Rect.Intersect(this.Rect);
                    if (intersection.IsEmptyWithTolerance())
                        continue;

                    if (platform.Rect.Left > this.Rect.Left)
                    {
                        if (platform.IsMovable)
                        {
                            var movable = (FPMovablePlatform)platform;
                            platform.Move((float)intersection.Width, 0.0f);
                            if (movable.CollisionLeftRight(game))
                            {
                                platform.Move((float)-intersection.Width, 0.0f);
                                game.MoveWorld((float)intersection.Width, 0.0f);
                                isColliding = true;
                            }
                        }
                        else
                        {
                            game.MoveWorld((float)intersection.Width, 0.0f);
                            isColliding = true;
                        }
                    }
                    else if (platform.Rect.Right < this.Rect.Right)
                    {
                        if (platform.IsMovable)
                        {
                            var movable = (FPMovablePlatform)platform;
                            platform.Move((float)-intersection.Width, 0.0f);
                            if (movable.CollisionLeftRight(game))
                            {
                                platform.Move((float)intersection.Width, 0.0f);
                                game.MoveWorld((float)-intersection.Width, 0.0f);
                                isColliding = true;
                            }
                        }
                        else
                        {
                            game.MoveWorld((float)-intersection.Width, 0.0f);
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
                    Rect intersection = platform.Rect.Intersect(this.Rect);
                    if (intersection.IsEmptyWithTolerance())
                        continue;

                    if (platform.Rect.Bottom < this.Rect.Bottom)
                    {
                        if (MoveY > 0.0f)
                            MoveY = 0.0f;

                        game.MoveWorld(0.0f, (float)-intersection.Height);
                        isColliding = true;
                    }
                    else if (MoveY < 0.0f)
                    {
                        if (platform.Rect.Top > this.Rect.Bottom - tolerance + MoveY)
                        {
                            MoveY = 0.0f;
                            jumping = false;
                            game.MoveWorld(0.0f, (float)intersection.Height);
                            isColliding = true;
                        }
                    }
                    else if (platform.Rect.Top > this.Rect.Bottom - tolerance + MoveY)
                    {
                        jumping = false;
                        game.MoveWorld(0.0f, (float)intersection.Height);
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
        }

        public void WriteToElement(XElement element)
        {
            element.WriteFloat("x", X);
            element.WriteFloat("y", Y);
        }
    }
}
