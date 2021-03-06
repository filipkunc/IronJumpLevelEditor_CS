#pragma once

namespace GLCanvas
{
	public ref class GLView : public System::Windows::Forms::UserControl
	{
	private:
		System::ComponentModel::Container^ components;

		HDC deviceContext;
		HGLRC glRenderingContext;
		GLView ^sharedContextView;
		PointF viewOffset;
	protected:
		~GLView();

		virtual void OnLoad(EventArgs ^e) override;
		virtual void OnSizeChanged(EventArgs ^e) override;
		virtual void OnPaintBackground(PaintEventArgs ^e) override;
		virtual void OnPaint(PaintEventArgs ^e) override;		
	public:
		GLView(void);

		void InitGL();
		void BeginGL();
		void EndGL();
		void ReshapeFlippedOrtho2D();
		void DrawGL();

		event EventHandler<CanvasEventArgs ^> ^PaintCanvas;
		
		property GLView ^SharedContextView
		{ 
			GLView ^get();
			void set(GLView ^value); 
		}

		property PointF ViewOffset
		{
			PointF get() { return viewOffset; }
			void set(PointF value) { viewOffset = value; }
		}
	private:
	#pragma region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		void InitializeComponent(void)
		{
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
		}
	#pragma endregion
	};
}

