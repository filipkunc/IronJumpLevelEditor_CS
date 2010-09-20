using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GLCanvas;
using System.Xml.Linq;
using System.Globalization;

namespace IronJumpLevelEditor_CS
{
    public static class Extensions
    {
        public static FPTexture CreateTexture(this FPCanvas canvas, Bitmap bitmap)
        {
            return canvas.CreateTexture(bitmap, false);
        }

        public static float ParseFloat(this XElement element, string child)
        {
            return float.Parse(element.Element(child).Value, CultureInfo.InvariantCulture);
        }

        public static int ParseInt(this XElement element, string child)
        {
            return int.Parse(element.Element(child).Value, CultureInfo.InvariantCulture);
        }

        public static void WriteFloat(this XElement element, string child, float value)
        {
            element.Add(new XElement(child, value.ToString(CultureInfo.InvariantCulture)));
        }

        public static void WriteInt(this XElement element, string child, int value)
        {
            element.Add(new XElement(child, value.ToString(CultureInfo.InvariantCulture)));
        }

        public static void AddRange<T>(this SortedSet<T> set, IEnumerable<T> objects)
        {
            foreach (var item in objects)
                set.Add(item);
        }
    }
}
