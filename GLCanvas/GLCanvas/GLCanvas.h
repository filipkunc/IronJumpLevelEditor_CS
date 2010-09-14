// GLCanvas.h

#pragma once

using namespace System;

namespace GLCanvas 
{
	ref class FPTexture;

	public ref class FPCanvas
	{
	public:
		void EnableTexturing();
		void DisableTexturing();
		void EnableBlend();
		void DisableBlend();

		void SetCurrentColor(Color color);
		void SetLineWidth(float width);
		void SetPointSize(float size);
		void DrawLine(PointF a, PointF b);
		void DrawPoint(PointF a);
		void FillRectangle(RectangleF rect);
		void DrawRectangle(RectangleF rect);

		FPTexture ^CreateTexture(Bitmap ^bitmap);		
	};
}
