#pragma once

namespace GLCanvas
{
	public ref class CanvasEventArgs : public EventArgs
	{
	private:
		Canvas ^canvas;
	public:
		CanvasEventArgs(Canvas ^canvas);

		property Canvas ^CanvasGL
		{ 
			Canvas ^get(); 
			void set(Canvas ^value); 
		}
	};
}
