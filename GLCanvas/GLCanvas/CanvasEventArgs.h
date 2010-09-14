#pragma once

namespace GLCanvas
{
	public ref class CanvasEventArgs : public EventArgs
	{
	private:
		FPCanvas ^canvas;
	public:
		CanvasEventArgs(FPCanvas ^canvas);

		property FPCanvas ^Canvas
		{ 
			FPCanvas ^get() { return canvas; }
			void set(FPCanvas ^value) { canvas = value; }
		}
	};
}
