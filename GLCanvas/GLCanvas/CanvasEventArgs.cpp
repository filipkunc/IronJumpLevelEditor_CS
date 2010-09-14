#include "StdAfx.h"
#include "GLCanvas.h"
#include "CanvasEventArgs.h"

namespace GLCanvas
{
	CanvasEventArgs::CanvasEventArgs(FPCanvas ^canvas)
	{
		this->canvas = canvas;
	}
}
