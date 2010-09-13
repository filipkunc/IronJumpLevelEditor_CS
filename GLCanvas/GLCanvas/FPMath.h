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
	};
}
