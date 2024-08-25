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
    public class FPElevator : FPGameObject
    {
        static FPTextureArray _elevatorTextures = null;

        public static FPTexture ElevatorTexture
        {
            get { return _elevatorTextures[0]; }
        }

        public static void InitTextures()
        {
            _elevatorTextures = new FPTextureArray();
            for (int i = 0; i < 3; i++)
            {
                _elevatorTextures.AddTexture(new FPTexture($"vytah0{i + 1}.png"));
            }
		}

        int animationCounter;
        int textureIndex;
        bool movingToEnd;
        List<FPGameObject> affectedObjects;
        FPElevatorEnd elevatorEnd;

        public float X { get; private set; }
        public float Y { get; private set; }
        public Rect Rect { get { return new Rect(X, Y, 32.0f * WidthSegments, 32.0f); } }
        public bool IsVisible { get; set; }
        public bool IsTransparent { get { return true; } }
        public bool IsPlatform { get { return true; } }
        public bool IsMovable { get { return false; } }
        public int WidthSegments { get; set; }
        public int HeightSegments { get { return 0; } set { } }
        public FPGameObject NextPart
        {
            get
            {
                if (elevatorEnd == null)
                    elevatorEnd = new FPElevatorEnd(this);
                return elevatorEnd;
            }
        }
        public float StartX { get; set; }
        public float StartY { get; set; }
        public float EndX { get; set; }
        public float EndY { get; set; }

        public FPElevator()
        {
            StartX = X = 0.0f;
            StartY = Y = 0.0f;
            EndX = 0.0f;
            EndY = 96.0f;
            IsVisible = true;
            textureIndex = 0;
            WidthSegments = 1;
            movingToEnd = true;
            affectedObjects = null;
            animationCounter = 0;
        }

        public void Move(float offsetX, float offsetY)
        {
            X += offsetX;
            Y += offsetY;
            StartX += offsetX;
            StartY += offsetY;
            if (elevatorEnd == null)
            {
                EndX += offsetX;
                EndY += offsetY;
            }
        }

        public void MoveCurrent(float offsetX, float offsetY)
        {
            X += offsetX;
            Y += offsetY;
        }

        public void ElevatorCollision(FPGameProtocol game, float diffX, float diffY)
        {
            FPPlayer player = (FPPlayer)game.Player;
            Rect playerRect = player.Rect;
            Rect moveRect = this.Rect.WithMove(diffX, diffY);

            if (diffY > 0.0f)
            {
                if (playerRect.IntersectsWithTolerance(moveRect))
                    diffY = 0.0f;
                else
                {
                    foreach (var gameObject in game.GameObjects)
                    {
                        if (gameObject.IsMovable)
                        {
                            if (gameObject.Rect.IntersectsWithTolerance(moveRect))
                            {
                                diffY = 0.0f;
                                break;
                            }
                        }
                    }
                }
            }

            if (Math.Abs(diffX) > 0.0f)
            {
                if (playerRect.IntersectsWithTolerance(moveRect))
                    diffX = 0.0f;
                else
                {
                    foreach (var gameObject in game.GameObjects)
                    {
                        if (gameObject.IsMovable)
                        {
                            if (gameObject.Rect.IntersectsWithTolerance(moveRect))
                            {
                                diffX = 0.0f;
                                break;
                            }
                        }
                    }
                }
            }

            playerRect = playerRect.WithHeight(playerRect.Height + FPPlayer.tolerance);

            if (playerRect.IntersectsWithTolerance(moveRect))
            {
                game.MoveWorld(-diffX, 0.0f);
                player.CollisionLeftRight(game);
                game.MoveWorld(0.0f, -diffY);
            }
            else
            {
                foreach (var gameObject in affectedObjects)
                {
                    moveRect = gameObject.Rect.WithMove(diffX, diffY);
                    if (playerRect.IntersectsWithTolerance(moveRect))
                    {
                        game.MoveWorld(-diffX, 0.0f);
                        player.CollisionLeftRight(game);
                        game.MoveWorld(0.0f, -diffY);
                        break;
                    }
                }
            }

            FPGameObject movableOnElevator = null;
            Rect movableRect;

            foreach (var movable in game.GameObjects)
            {
                if (movable.IsMovable)
                {
                    moveRect = this.Rect.WithMove(diffX, diffY);
                    movableRect = movable.Rect;
					movableRect = movableRect.WithHeight(movableRect.Height + FPPlayer.tolerance);
                    if (movableRect.Bottom < moveRect.Bottom && movableRect.IntersectsWithTolerance(moveRect))
                    {
                        movableOnElevator = movable;
                        break;
                    }
                    else
                    {
                        foreach (var gameObject in affectedObjects)
                        {
                            moveRect = gameObject.Rect.WithMove(diffX, diffY);
                            if (movableRect.Bottom < moveRect.Bottom && movableRect.IntersectsWithTolerance(moveRect))
                            {
                                movableOnElevator = movable;
                                break;
                            }
                        }
                    }
                }
            }

            if (movableOnElevator != null)
            {
                movableRect = movableOnElevator.Rect.WithMove(diffX, diffY);
                movableOnElevator.Move(diffX, 0.0f);
                if (playerRect.IntersectsWithTolerance(movableRect))
                {
                    game.MoveWorld(-diffX, 0.0f);
                    player.CollisionLeftRight(game);
                }
                movableOnElevator.Move(0.0f, diffY);
                if (playerRect.IntersectsWithTolerance(movableRect))
                {
                    game.MoveWorld(0.0f, -diffY);
                }
            }

            X += diffX;
            Y += diffY;
            foreach (var gameObject in affectedObjects)
                gameObject.Move(diffX, diffY);
        }

        void InitAffectedObjectsIfNeeded(FPGameProtocol game)
        {
            if (affectedObjects == null)
            {
                affectedObjects = new List<FPGameObject>();
                Rect selfRect = this.Rect;

                foreach (var gameObject in game.GameObjects)
                {
                    if (gameObject == this)
                        continue;

					Rect gameObjectRect = gameObject.Rect;
					gameObjectRect = gameObjectRect.WithHeight(gameObjectRect.Height + FPPlayer.tolerance);
                    if (gameObjectRect.Intersects(selfRect))
                        affectedObjects.Add(gameObject);
                }

                foreach (var gameObject in game.GameObjects)
                {
                    if (gameObject == this)
                        continue;

                    if (affectedObjects.Contains(gameObject))
                        continue;

					Rect gameObjectRect = gameObject.Rect;
					gameObjectRect = gameObjectRect.WithHeight(gameObjectRect.Height + FPPlayer.tolerance);

					foreach (var affectedObject in affectedObjects)
                    {
                        if (gameObjectRect.Intersects(affectedObject.Rect))
                            affectedObjects.Add(gameObject);
                    }
                }
            }
        }

        public void Update(FPGameProtocol game)
        {
            const float speed = 2.0f;

            float diffX, diffY;

            if (movingToEnd)
            {
                diffX = EndX - X;
                diffY = EndY - Y;
            }
            else
            {
                diffX = StartX - X;
                diffY = StartY - Y;
            }

            diffX = FPMath.fabsminf(diffX, speed);
            diffY = FPMath.fabsminf(diffY, speed);

            if (Math.Abs(diffX) < 0.1f && Math.Abs(diffY) < 0.1f)
            {
                movingToEnd = !movingToEnd;
            }

            InitAffectedObjectsIfNeeded(game);
            ElevatorCollision(game, diffX, diffY);

            if (textureIndex > 2)
                textureIndex = 2;

            if (textureIndex < 0)
                textureIndex = 0;

            if (diffY < 0.0f)
            {
                if (++animationCounter > 2)
                {
                    animationCounter = 0;
                    if (++textureIndex >= 2)
                        textureIndex = 2;
                }
            }
            else if (diffY > 0.0f)
            {
                if (++animationCounter > 2)
                {
                    animationCounter = 0;
                    if (--textureIndex < 0)
                        textureIndex = 0;
                }
            }
            else
            {
                textureIndex = 1;
            }
        }

        public bool Draw(FPDrawBuilder drawBuilder, Rect bounds)
        {
            if (Rect.Intersects(bounds))
            {
                drawBuilder.AddSprite(_elevatorTextures[textureIndex], X, Y, WidthSegments);
                return true;
            }
            return false;
        }

        public FPGameObject Duplicate(float offsetX, float offsetY)
        {
            FPElevator duplicated = new FPElevator();
            duplicated.WidthSegments = WidthSegments;
            duplicated.Move(X + offsetX, Y + offsetY);
            duplicated.EndX = EndX + offsetX;
            duplicated.EndY = EndY + offsetY;
            return duplicated;
        }

        public void InitFromElement(XElement element)
        {
            StartX = X = element.ParseFloat("x");
            StartY = Y = element.ParseFloat("y");
            EndX = element.ParseFloat("endX");
            EndY = element.ParseFloat("endY");
            WidthSegments = element.ParseInt("widthSegments");
        }

        public void WriteToElement(XElement element)
        {
            element.WriteFloat("x", X);
            element.WriteFloat("y", Y);
            element.WriteFloat("endX", EndX);
            element.WriteFloat("endY", EndY);
            element.WriteInt("widthSegments", WidthSegments);
        }
    }

    public class FPElevatorEnd : FPGameObject
    {
        FPElevator elevatorStart;

        public FPElevatorEnd(FPElevator elevatorStart)
        {
            this.elevatorStart = elevatorStart;
        }

        public float X { get { return elevatorStart.EndX; } }
        public float Y { get { return elevatorStart.EndY; } }
        public Rect Rect { get { return new Rect(X, Y, WidthSegments * 32.0f, 32.0f); } }
        public bool IsVisible { get { return true; } set { } }
        public bool IsTransparent { get { return true; } }
        public bool IsPlatform { get { return true; } }
        public bool IsMovable { get { return false; } }
        public int WidthSegments { get { return elevatorStart.WidthSegments; } set { elevatorStart.WidthSegments = value; } }
        public int HeightSegments { get { return 0; } set { } }
        public FPGameObject NextPart { get { return elevatorStart; } }

        public void Move(float offsetX, float offsetY)
        {
            elevatorStart.EndX += offsetX;
            elevatorStart.EndY += offsetY;
        }

        public FPGameObject Duplicate(float offsetX, float offsetY)
        {
            return null;
        }

        public void Update(FPGameProtocol game)
        {
            
        }

        public bool Draw(FPDrawBuilder drawBuilder, Rect bounds)
        {
            bool endIsVisible = false;
            if (Rect.Intersects(bounds))
            {
                drawBuilder.AddSprite(FPElevator.ElevatorTexture, X, Y, WidthSegments, heightSegments: 1, opacity: 100);
                endIsVisible = true;
            }

            var start = elevatorStart.Rect;
            var end = this.Rect;

			drawBuilder.Context.DrawLine(new Pen(Brushes.Green), start.MiddlePoint(), end.MiddlePoint());
            return endIsVisible;
        }

        public void InitFromElement(XElement element)
        {
            
        }

        public void WriteToElement(XElement element)
        {
            
        }
    }
}
