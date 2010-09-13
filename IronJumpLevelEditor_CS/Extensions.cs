using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GLCanvas;

namespace IronJumpLevelEditor_CS
{
    public static class Extensions
    {
        public static void Draw(this Texture texture, PointF position, int widthSegments, int heightSegments)
        {
            for (int y = 0; y < heightSegments; y++)
            {
                for (int x = 0; x < widthSegments; x++)
                {
                    texture.Draw(new PointF(position.X + x * texture.Width, position.Y + y * texture.Height));
                }
            }
        }
    }
}
