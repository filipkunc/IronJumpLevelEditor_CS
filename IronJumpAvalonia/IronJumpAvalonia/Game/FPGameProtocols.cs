using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Avalonia;
using Avalonia.Media;

namespace IronJumpAvalonia.Game
{
    public interface FPGameProtocol
    {
        Vector InputAcceleration { get; set; }
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
        Rect Rect { get; }
        bool IsVisible { get; set; }
        bool IsTransparent { get; }
        bool IsPlatform { get; }
        bool IsMovable { get; }

        void Move(float offsetX, float offsetY);
        bool Draw(FPDrawBuilder drawBuilder, Rect bounds);

        FPGameObject Duplicate(float offsetX, float offsetY);

        // optional
        int WidthSegments { get; set; }
        int HeightSegments { get; set; }
        FPGameObject NextPart { get; }

        void Update(FPGameProtocol game);

        void InitFromElement(XElement element);
        void WriteToElement(XElement element);
    }
}
