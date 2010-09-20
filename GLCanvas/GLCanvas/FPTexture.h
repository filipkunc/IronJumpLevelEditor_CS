#pragma once

typedef struct
{
	GLfloat x, y;
	GLshort s, t;
} FPVertex;

typedef struct
{
	GLfloat x, y;
	GLfloat s, t;
} FPAtlasVertex;

#define kMaxVertices 8192

extern FPAtlasVertex globalVertexBuffer[kMaxVertices];

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
		void Draw(PointF position, int widthSegments, int heightSegments);
		void Draw(PointF position, float rotation);
		void DrawText(String ^text, PointF pt, float horizontalTileCount, float verticalTileCount, float tileSize, float spacing);
	};
}
