#pragma once

namespace GLCanvas
{
	public ref class FPTexture
	{
	private:
		unsigned int textureID;
		float width;
		float height;
	public:
		property float Width { float get() { return width; } }
		property float Height { float get() { return height; } }
		
		FPTexture(unsigned int textureID, float width, float height);

		void Draw(PointF position);
		void Draw(PointF position, float rotation);
	};
}
