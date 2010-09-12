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
		Texture(unsigned int textureID, float width, float height);

		void Draw(PointF position);
		void Draw(PointF position, float rotation);
	};
}
