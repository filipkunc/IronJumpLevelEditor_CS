using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GLCanvas;

namespace IronJumpLevelEditor_CS.PortedClasses
{
    public interface FPGameProtocol
    {
        PointF InputAcceleration { get; set; }
        float Width { get; }
        float Height { get; }

        FPGameObject Player { get; }
        List<FPGameObject> GameObjects { get; }

        void MoveWorld(float x, float y);
    }

    public interface FPGameObject
    {
        float X { get; }
        float Y { get; }
        RectangleF Rect { get; }
        bool IsVisible { get; set; }
        bool IsTransparent { get; }
        bool IsPlatform { get; }
        bool IsMovable { get; }

        void Move(float offsetX, float offsetY);
        void Draw(Canvas canvas);

        FPGameObject Duplicate(float offsetX, float offsetY);

        // optional
        int WidthSegments { get; set; }
        int HeightSegments { get; set; }
        FPGameObject NextPart { get; }
        float MoveY { get; set; }

        void Update(FPGameProtocol game);
        bool CollisionLeftRight(FPGameProtocol game);
    }
}
