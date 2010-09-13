// This is the main DLL file.

#include "stdafx.h"
#include "Texture.h"
#include "GLCanvas.h"

void CreateTexture(GLubyte *data, int components, GLuint *textureID, int width, int height, bool convertToAlpha)
{
	glEnable(GL_TEXTURE_2D);
	glGenTextures(1, textureID);
	glBindTexture(GL_TEXTURE_2D, *textureID);

	if (convertToAlpha)
	{
		GLubyte *alphaData = (GLubyte *)malloc(width * height);
		for (int i = 0; i < width * height; i++)
			alphaData[i] = data[i * components];

		glTexImage2D(GL_TEXTURE_2D, 0, GL_ALPHA, width, height, 0, GL_ALPHA, GL_UNSIGNED_BYTE, alphaData);
		free(alphaData);
	}
	else 
	{
		if (components == 3)
			glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, width, height, 0, GL_RGB, GL_UNSIGNED_BYTE, data);
		else if (components == 4)
		{
			GLubyte *rgbaData = (GLubyte *)malloc(width * height * components);
			for (int i = 0; i < width * height; i++)
			{
				rgbaData[i * components + 0] = data[i * components + 2];
				rgbaData[i * components + 1] = data[i * components + 1];
				rgbaData[i * components + 2] = data[i * components + 0];
				rgbaData[i * components + 3] = data[i * components + 3];
			}
			
			glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, width, height, 0, GL_RGBA, GL_UNSIGNED_BYTE, rgbaData);
			free(rgbaData);
		}
		else
			throw "Unsupported texture format";
	}

	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP);
}


namespace GLCanvas
{
	void Canvas::EnableTexturing()
	{
		glEnable(GL_TEXTURE_2D);
	}

	void Canvas::DisableTexturing()
	{
		glDisable(GL_TEXTURE_2D);
	}

	void Canvas::EnableBlend()
	{
		glEnable(GL_BLEND);
	}

	void Canvas::DisableBlend()
	{
		glDisable(GL_BLEND);
	}

	void Canvas::SetCurrentColor(Color color)
	{
		glColor4ub(color.R, color.G, color.B, color.A);
	}

	void Canvas::SetLineWidth(float width)
	{
		glLineWidth(width);
	}

	void Canvas::SetPointSize(float size)
	{
		glPointSize(size);
	}

	void Canvas::DrawPoint(PointF a)
	{
		glBegin(GL_POINTS);
		glVertex2f(a.X, a.Y);
		glEnd();
	}

	void Canvas::DrawLine(PointF a, PointF b)
	{
		glBegin(GL_LINES);
		glVertex2f(a.X, a.Y);
		glVertex2f(b.X, b.Y);
		glEnd();
	}

	void Canvas::FillRectangle(RectangleF rect)
	{
		glRectf(rect.X, rect.Y, rect.X + rect.Width, rect.Y + rect.Height);
	}

	void Canvas::DrawRectangle(RectangleF rect)
	{
		glPolygonMode(GL_FRONT_AND_BACK, GL_LINE);		
		glRectf(rect.X, rect.Y, rect.X + rect.Width, rect.Y + rect.Height);
		glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);
	}

	Texture ^Canvas::CreateTexture(Bitmap ^bitmap)
	{
		System::Drawing::Rectangle rect = System::Drawing::Rectangle(Point::Empty, bitmap->Size);
        BitmapData ^data = bitmap->LockBits(rect, ImageLockMode::ReadOnly, PixelFormat::Format32bppArgb);

		unsigned int textureID;
		::CreateTexture((GLubyte *)data->Scan0.ToPointer(), 4, &textureID, rect.Width, rect.Height, false);
		
		bitmap->UnlockBits(data);

		return gcnew Texture(textureID, (float)rect.Width, (float)rect.Height);
	}	
}