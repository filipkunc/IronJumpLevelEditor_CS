#include "StdAfx.h"
#include "FPTexture.h"

typedef struct
{
	GLfloat x, y;
	GLshort s, t;
} FPVertex;

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
}
