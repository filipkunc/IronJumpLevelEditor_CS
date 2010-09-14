#pragma once

namespace GLCanvas
{
	public ref class FPMath
	{
	public:
		FPMath(void);

		static float sinf(float x) { return ::sinf(x); }
		static float cosf(float x) { return ::cosf(x); }
		static float fmodf(float x, float y) { return ::fmodf(x, y); }
		static float fabsmaxf(float n, float max) {	return n > 0.0f ? std::max(n, max) : std::min(n, -max); }
		static float fabsminf(float n, float min) {	return n > 0.0f ? std::min(n, min) : std::max(n, -min); }
		static float flerpf(float a, float b, float w) { return a * (1.0f - w) + b * w; }
	};
}
