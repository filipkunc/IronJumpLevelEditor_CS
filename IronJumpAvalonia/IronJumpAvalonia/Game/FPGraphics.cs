using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJumpAvalonia.Game
{
    public static class FPGraphics
    {
        const float rectTolerance = 0.1f;

        public static Rect RectangleFromPoints(this Point a, Point b)
        {
            var x1 = Math.Min(a.X, b.X);
            var y1 = Math.Min(a.Y, b.Y);
            var x2 = Math.Max(a.X, b.X);
            var y2 = Math.Max(a.Y, b.Y);

            return new Rect(x1, y1, x2 - x1, y2 - y1);
        }

        public static Point MiddlePoint(this Rect rect)
        {
            return new Point(rect.X + rect.Width / 2.0f, rect.Y + rect.Height / 2.0f);
        }

        public static Rect WithMove(this Rect rect, float moveX, float moveY)
        {
            rect.Position.Deconstruct(out double x, out double y);
            rect.Size.Deconstruct(out double w, out double h);

            if (moveX < 0.0f)
            {
                x += moveX;
                w -= moveX;
            }
            else
            {
                w += moveX;
            }

            if (moveY < 0.0f)
            {
                y += moveY;
                h -= moveY;
            }
            else
            {
                h += moveY;
            }

            return new Rect(x, y, w, h);
        }

        public static bool IntersectsWithTolerance(this Rect a, Rect b)
        {
            Rect intersection = a.Intersect(b);
            if (intersection.IsEmptyWithTolerance())
                return false;
            return true;
        }

        public static bool IsEmptyWithTolerance(this Rect a)
        {
            if (a.Width < rectTolerance || a.Height < rectTolerance)
                return true;
            return false;
        }
    }
}
