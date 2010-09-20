#include "StdAfx.h"
#include "FPTexture.h"

FPAtlasVertex globalVertexBuffer[kMaxVertices];

namespace GLCanvas
{
	FPTexture::FPTexture(unsigned int textureID, float width, float height)
	{
		this->textureID = textureID;
		this->width = width;
		this->height = height;
	}

	void FPTexture::Draw(PointF position)
	{
		const float x = position.X;
		const float y = position.Y;

		const FPVertex vertices[] = 
		{
			{ x,			y,				0, 0, }, // 0
			{ x + width,	y,				1, 0, }, // 1
			{ x,			y + height,		0, 1, }, // 2
			{ x + width,	y + height,		1, 1, }, // 3
		};	

		glEnable(GL_TEXTURE_2D);
		glBindTexture(GL_TEXTURE_2D, textureID);
		glEnableClientState(GL_VERTEX_ARRAY);
		glVertexPointer(2, GL_FLOAT, sizeof(FPVertex), &vertices->x);	
		glEnableClientState(GL_TEXTURE_COORD_ARRAY);
		glTexCoordPointer(2, GL_SHORT, sizeof(FPVertex), &vertices->s);
		glDrawArrays(GL_TRIANGLE_STRIP, 0, 4);
	}

	void FPTexture::Draw(PointF position, int widthSegments, int heightSegments)
	{
		for (int y = 0; y < heightSegments; y++)
        {
            for (int x = 0; x < widthSegments; x++)
            {
                Draw(PointF(position.X + x * Width, position.Y + y * Height));
            }
        }
	}

	void FPTexture::Draw(PointF position, float rotation)
	{
		glPushMatrix();
		glTranslatef(position.X, position.Y, 0.0f);	
		glTranslatef(width / 2.0f, height / 2.0f, 0.0f);
		glRotatef(rotation, 0, 0, 1);	
		glTranslatef(-width / 2.0f, -height / 2.0f, 0.0f);
		Draw(PointF::Empty);
		glPopMatrix();
	}

	void FPTexture::DrawText(String ^text, PointF pt, float horizontalTileCount, float verticalTileCount, float tileSize, float spacing)
	{
		if (text == nullptr)
			return;
	
		float texCoordX = 1.0f / horizontalTileCount;
		float texCoordY = 1.0f / verticalTileCount;	
		int length = text->Length;
		
		FPAtlasVertex *vertexPtr = globalVertexBuffer;
		int verticesUsed = 0;
	
		for (int i = 0; i < length; i++)
		{
			int character = text[i];
			if (character != ' ')
			{
				int y = character / 16;
				int x = character - (y * 16);
			
				const FPAtlasVertex vertices[] = 
				{
					{ pt.X,				pt.Y,				x * texCoordX,			y * texCoordY, },			// 0
					{ pt.X + tileSize,	pt.Y,				(x + 1) * texCoordX,	y *	texCoordY, },			// 1
					{ pt.X,				pt.Y + tileSize,	x * texCoordX,			(y + 1) * texCoordY, },		// 2
				
					{ pt.X,				pt.Y + tileSize,	x * texCoordX,			(y + 1) * texCoordY, },		// 2
					{ pt.X + tileSize,	pt.Y + tileSize,	(x + 1) * texCoordX,	(y + 1) * texCoordY, },		// 3
					{ pt.X + tileSize,	pt.Y,				(x + 1) * texCoordX,	y *	texCoordY, },			// 1
				};
			
				verticesUsed += 6;
				if (verticesUsed > kMaxVertices)
				{
					throw gcnew Exception("verticesUsed >= kMaxVertices");
				}
			
				memcpy(vertexPtr, vertices, sizeof(vertices));
				vertexPtr += 6;
			}
			pt.X += spacing;
		}
	
		if (verticesUsed > 0)
		{
			glEnable(GL_TEXTURE_2D);
			glBindTexture(GL_TEXTURE_2D, textureID);
			glVertexPointer(2, GL_FLOAT, sizeof(FPAtlasVertex), &globalVertexBuffer->x);
			glEnableClientState(GL_VERTEX_ARRAY);
			glTexCoordPointer(2, GL_FLOAT, sizeof(FPAtlasVertex), &globalVertexBuffer->s);
			glEnableClientState(GL_TEXTURE_COORD_ARRAY);
			glDrawArrays(GL_TRIANGLES, 0, verticesUsed);
		}
	}
}
