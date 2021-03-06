#include "StdAfx.h"
#include "GLCanvas.h"
#include "CanvasEventArgs.h"
#include "GLView.h"

namespace GLCanvas
{
	GLView::GLView(void)
	{
		deviceContext = nullptr;
		glRenderingContext = nullptr;
		sharedContextView = nullptr;
		viewOffset.X = 0.0f;
		viewOffset.Y = 0.0f;
	}

	GLView::~GLView()
	{
		EndGL();
		if (components)
		{
			delete components;
		}
	}

	GLView ^GLView::SharedContextView::get()
	{
		return sharedContextView;
	}

	void GLView::SharedContextView::set(GLView ^value)
	{
		sharedContextView = value;
	}

	#pragma region OnEvents

	void GLView::OnLoad(EventArgs ^e)
	{
		UserControl::OnLoad(e);
		InitGL();
	}

	void GLView::OnSizeChanged(EventArgs ^e)
	{
		UserControl::OnSizeChanged(e);
		Invalidate();
	}

	void GLView::OnPaint(PaintEventArgs ^e)
	{
		if (this->DesignMode)
			return;

		if (!deviceContext || !glRenderingContext)
			return;

		BeginGL();
		DrawGL();
		SwapBuffers(deviceContext);
		EndGL();
	}

	void GLView::OnPaintBackground(PaintEventArgs ^e)
	{
		if (this->DesignMode)
			UserControl::OnPaintBackground(e);
	}

	#pragma endregion

	void GLView::InitGL()
	{
		if (this->DesignMode)
			return;

		deviceContext = GetDC((HWND)this->Handle.ToPointer());
		// CAUTION: Not doing the following SwapBuffers() on the DC will
		// result in a failure to subsequently create the RC.
		SwapBuffers(deviceContext);

		//Get the pixel format		
		PIXELFORMATDESCRIPTOR pfd;
		ZeroMemory(&pfd, sizeof(pfd));
		pfd.nSize = sizeof(pfd);
		pfd.nVersion = 1;
		pfd.dwFlags = PFD_DRAW_TO_WINDOW | PFD_SUPPORT_OPENGL | PFD_DOUBLEBUFFER;
		pfd.iPixelType = PFD_TYPE_RGBA;
		pfd.cColorBits = 32;
		pfd.cDepthBits = 0;
		pfd.iLayerType = PFD_MAIN_PLANE;

		int pixelFormatIndex = ChoosePixelFormat(deviceContext, &pfd);
		if (pixelFormatIndex == 0)
		{
			Trace::WriteLine("Unable to retrieve pixel format");
			return;
		}

		if (SetPixelFormat(deviceContext, pixelFormatIndex, &pfd) == 0)
		{
			Trace::WriteLine("Unable to set pixel format");
			return;
		}

		wglMakeCurrent(0, 0);

		//Create rendering context
		glRenderingContext = wglCreateContext(deviceContext);
		if (sharedContextView != nullptr)
		{
			wglShareLists(sharedContextView->glRenderingContext, glRenderingContext);
		}
		if (!glRenderingContext)
		{
			Trace::WriteLine("Unable to get rendering context");
			return;
		}
		if (wglMakeCurrent(deviceContext, glRenderingContext) == 0)
		{
			Trace::WriteLine("Unable to make rendering context current");
			return;
		}
	}

	void GLView::BeginGL()
	{
		wglMakeCurrent(deviceContext, glRenderingContext);
	}

	void GLView::EndGL()
	{
		wglMakeCurrent(NULL, NULL);
	}

	void GLView::ReshapeFlippedOrtho2D()
	{
		System::Drawing::Rectangle rect = this->ClientRectangle;

		glViewport(0, 0, rect.Width, rect.Height);

		glMatrixMode(GL_PROJECTION);
		glLoadIdentity();
		glOrtho(0, rect.Width, 0, rect.Height, -1.0f, 1.0f);

		glMatrixMode(GL_MODELVIEW);
		glLoadIdentity();

		glTranslatef((float)-rect.X, (float)rect.Y + (float)rect.Height, 0);
		glScalef(1, -1, 1);

		glTranslatef(viewOffset.X, viewOffset.Y, 0.0f);
	}

	void GLView::DrawGL()
	{
		ReshapeFlippedOrtho2D();

		glClearColor(55.0f / 255.0f, 60.0f / 255.0f, 89.0f / 255.0f, 1.0f);
		glEnable(GL_TEXTURE_2D);
		glEnable(GL_BLEND);
		glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);

		glClear(GL_COLOR_BUFFER_BIT);

		CanvasEventArgs ^args = gcnew CanvasEventArgs(gcnew FPCanvas());
		PaintCanvas(this, args);		
	}
}
