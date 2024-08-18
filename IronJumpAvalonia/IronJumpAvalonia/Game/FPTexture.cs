using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Skia;
using SkiaSharp;
using Avalonia.Rendering.SceneGraph;
using System.IO;

namespace IronJumpAvalonia.Game
{
	public class FPTextureDrawOp : ICustomDrawOperation
	{
		SKBitmap _bitmap;
		Rect _destRect;
		int _opacity;

		static SKColorSpace _colorSpace = SKColorSpace.CreateSrgb();

		public FPTextureDrawOp(SKBitmap bitmap, Rect destRect, int opacity)
		{
			_bitmap = bitmap;
			_destRect = destRect;
			_opacity = opacity;
		}

		public Rect Bounds => _destRect;

		public void Dispose() { }

		public bool Equals(ICustomDrawOperation? other) => false;

		public bool HitTest(Point p) => false;

		public void Render(ImmediateDrawingContext context)
		{
			var leaseFeature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
			if (leaseFeature == null)
				return;

			using var lease = leaseFeature.Lease();
			var canvas = lease.SkCanvas;

			canvas.Save();

			using (SKPaint paint = new SKPaint())
			{
				SKRect destRect = new SKRect((float)_destRect.Left, (float)_destRect.Top, (float)_destRect.Right, (float)_destRect.Bottom);

				var shader = SKShader.CreateBitmap(_bitmap, SKShaderTileMode.Repeat, SKShaderTileMode.Repeat,
					SKMatrix.CreateTranslation(destRect.Left, destRect.Top));
				paint.Shader = shader;
				if (_opacity != 255)
				{
					paint.SetColor(new SKColorF(1.0f, 1.0f, 1.0f, _opacity / 255.0f), _colorSpace);
				}
				canvas.DrawRect(destRect, paint);
			}

			canvas.Restore();
		}
	}


	public class FPTexture
	{
		Bitmap _bitmap;
		SKBitmap _skBitmap;

		public Size Size => _bitmap.Size;


		public FPTexture(string fileName)
		{
			using var stream = AssetLoader.Open(new Uri($"avares://IronJumpAvalonia/Assets/{fileName}"));
			using MemoryStream memory = new MemoryStream();
			stream.CopyTo(memory);
			memory.Seek(0, SeekOrigin.Begin);
			_bitmap = new Bitmap(memory);
			memory.Seek(0, SeekOrigin.Begin);
			_skBitmap = SKBitmap.Decode(memory);
		}

		public void Draw(DrawingContext context, float X, float Y, int widthSegments = 1, int heightSegments = 1, int opacity = 255)
		{
			if (widthSegments == 1 && heightSegments == 1)
			{
				using var opacityState = context.PushOpacity(opacity / 255.0);
				context.DrawImage(_bitmap, new Rect(X, Y, Size.Width, Size.Height));
			}
			else
			{
				var destRect = new Rect(X, Y, Size.Width * widthSegments, Size.Height * heightSegments);
				context.Custom(new FPTextureDrawOp(_skBitmap, destRect, opacity));
			}
		}

		public void Draw(DrawingContext context, float X, float Y, float Rotation)
		{
			var t1 = Matrix.CreateTranslation(-Size.Width / 2.0, -Size.Height / 2);
			var t2 = Matrix.CreateTranslation(X + Size.Width / 2.0, Y + Size.Height / 2);
			var r = Matrix.CreateRotation(Rotation * Math.PI / 180.0);
			using var state = context.PushTransform(t1 * r * t2);
			context.DrawImage(_bitmap, new Rect(0, 0, Size.Width, Size.Height));
		}
	}
}
