#include "StdAfx.h"
#include "GLCanvas.h"
#include "CanvasEventArgs.h"

namespace GLCanvas
{
	CanvasEventArgs::CanvasEventArgs(Canvas ^canvas)
	{
		this->canvas = canvas;
	}

	Canvas ^CanvasEventArgs::CanvasGL::get()
	{
		return canvas;
	}

	void CanvasEventArgs::CanvasGL::set(Canvas ^value)
	{
		canvas = value;
	}
}