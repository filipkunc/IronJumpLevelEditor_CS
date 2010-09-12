// GLCanvas.h

#pragma once

using namespace System;

namespace GLCanvas 
{
	ref class Texture;

	public ref class Canvas
	{
	public:
		void EnableTexturing();
		void DisableTexturing();
		void EnableBlend();
		void DisableBlend();

		void SetCurrentColor(Color color);
		void DrawLine(Point a, Point b);
		void FillRectangle(System::Drawing::Rectangle rect);
		void DrawRectangle(System::Drawing::Rectangle rect);

		Texture ^CreateTexture(Bitmap ^bitmap);		
	};
}
