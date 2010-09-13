#pragma once

namespace GLCanvas
{
	public ref class Texture
	{
	private:
		unsigned int textureID;
		float width;
		float height;
	public:
		property float Width { float get() { return width; } }
		property float Height { float get() { return height; } }
		
		Texture(unsigned int textureID, float width, float height);

		void Draw(PointF position);
		void Draw(PointF position, float rotation);
	};
}
