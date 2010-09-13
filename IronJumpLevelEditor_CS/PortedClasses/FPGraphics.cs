using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace IronJumpLevelEditor_CS.PortedClasses
{
    public static class FPGraphics
    {
        const float rectTolerance = 0.1f;

        public static RectangleF RectangleFromPoints(this PointF a, PointF b)
        {
            float x1 = Math.Min(a.X, b.X);
            float y1 = Math.Min(a.Y, b.Y);
            float x2 = Math.Max(a.X, b.X);
            float y2 = Math.Max(a.Y, b.Y);

            return new RectangleF(x1, y1, x2 - x1, y2 - y1);
        }

        public static PointF MiddlePoint(this RectangleF rect)
        {
            return new PointF(rect.X + rect.Width / 2.0f, rect.Y + rect.Height / 2.0f);
        }

        public static RectangleF WithMove(this RectangleF rect, float moveX, float moveY)
        {
            if (moveX < 0.0f)
            {
                rect.X += moveX;
                rect.Width -= moveX;
            }
            else
            {
                rect.Width += moveX;
            }

            if (moveY < 0.0f)
            {
                rect.Y += moveY;
                rect.Height -= moveY;
            }
            else
            {
                rect.Height += moveY;
            }

            return rect;
        }

        public static bool IntersectsWithTolerance(this RectangleF a, RectangleF b)
        {
            RectangleF intersection = RectangleF.Intersect(a, b);
            if (intersection.IsEmptyWithTolerance())
                return false;
            return true;
        }

        public static bool IsEmptyWithTolerance(this RectangleF a)
        {
            if (a.Width < rectTolerance || a.Height < rectTolerance)
                return true;
            return false;
        }
    }
}
