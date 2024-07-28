using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronJumpAvalonia.Game
{
	public static class FPMath
	{
		public static float sinf(float x) { return MathF.Sin(x); }
		public static float cosf(float x) { return MathF.Cos(x); }
		public static float fmodf(float x, float y) { return x % y; }
		public static float fabsmaxf(float n, float max) { return n > 0.0f ? Math.Max(n, max) : Math.Min(n, -max); }
		public static float fabsminf(float n, float min) { return n > 0.0f ? Math.Min(n, min) : Math.Max(n, -min); }
		public static float flerpf(float a, float b, float w) { return a * (1.0f - w) + b * w; }
	}
}
