using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IronJumpAvalonia.Game
{
	public static class Extensions
	{
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
