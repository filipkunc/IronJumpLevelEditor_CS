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
using System.Collections;

namespace IronJumpAvalonia.Game
{
	public class FPTexture
	{
		SKRect _tile;

		public SKRect Tile => _tile;
		public SKSize Size => _tile.Size;

		

		public FPTexture(string fileName)
		{
			switch (fileName.ToLowerInvariant().Split(".").FirstOrDefault())
			{
				case "ball": _tile = SKRect.Create(1.0f, 1.0f, 32.0f, 32.0f); break;
				case "diamond": _tile = SKRect.Create(34.0f, 1.0f, 32.0f, 32.0f); break;
				case "marbleblue": _tile = SKRect.Create(34.0f, 34.0f, 32.0f, 32.0f); break;
				case "movable": _tile = SKRect.Create(100.0f, 1.0f, 32.0f, 32.0f); break;
				case "plos_marble": _tile = SKRect.Create(133.0f, 1.0f, 32.0f, 32.0f); break;
				case "trampoline01": _tile = SKRect.Create(166.0f, 1.0f, 64.0f, 32.0f); break;
				case "trampoline02": _tile = SKRect.Create(231.0f, 1.0f, 64.0f, 32.0f); break;
				case "trampoline03": _tile = SKRect.Create(296.0f, 1.0f, 64.0f, 32.0f); break;
				case "vytah01": _tile = SKRect.Create(361.0f, 1.0f, 32.0f, 32.0f); break;
				case "vytah02": _tile = SKRect.Create(394.0f, 1.0f, 32.0f, 32.0f); break;
				case "vytah03": _tile = SKRect.Create(427.0f, 1.0f, 32.0f, 32.0f); break;
				case "magnet": _tile = SKRect.Create(460.0f, 1.0f, 32.0f, 32.0f); break;
				case "speed_symbol": _tile = SKRect.Create(1.0f, 34.0f, 32.0f, 32.0f); break;
				case "exit": _tile = SKRect.Create(67.0f, 34.0f, 64.0f, 64.0f); break;
				case "speed": _tile = SKRect.Create(132.0f, 34.0f, 64.0f, 64.0f); break;
			}
		}
	}

	public class FPDrawBuilder
	{
		static SKImage _atlasImage = null;

		List<SKRect> _tiles = new List<SKRect>();
		List<SKRotationScaleMatrix> _transforms = new List<SKRotationScaleMatrix>();
		List<SKColor> _colors = new List<SKColor>();

		DrawingContext _context;
		Rect _bounds;

		public DrawingContext Context => _context;
		public Rect Bounds => _bounds;

		class AtlasDrawOperation : ICustomDrawOperation
		{
			SKImage _image;
			Rect _bounds;
			SKRect[] _tiles;
			SKRotationScaleMatrix[] _transforms;
			SKColor[] _colors;

			public AtlasDrawOperation(SKImage image, Rect bounds, SKRect[] tiles, SKRotationScaleMatrix[] transforms, SKColor[] colors)
			{
				_image = image;
				_bounds = bounds;
				_tiles = tiles;
				_transforms = transforms;
				_colors = colors;
			}

			public Rect Bounds => _bounds;

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
					canvas.DrawAtlas(_image, _tiles, _transforms, _colors, SKBlendMode.Modulate, paint);
				}

				canvas.Restore();
			}
		}

		public FPDrawBuilder(DrawingContext context, Rect bounds)
		{
			_context = context;
			_bounds = bounds;
		}

		public void AddSprite(FPTexture texture, float X, float Y, int widthSegments = 1, int heightSegments = 1, int opacity = 255)
		{
			var tile = texture.Tile;
			float w = texture.Size.Width;
			float h = texture.Size.Height;
			for (int y = 0; y < heightSegments; y++)
			{
				for (int x = 0; x < widthSegments; x++)
				{
					float tx = X + x * w;
					float ty = Y + y * h;

					if (tx > _bounds.Right || tx + w < _bounds.Left ||
						ty > _bounds.Bottom || ty + w < _bounds.Top)
						continue;

					_tiles.Add(tile);
					_transforms.Add(SKRotationScaleMatrix.CreateTranslation(tx, ty));
					_colors.Add(new SKColor(255, 255, 255, 255));
				}
			}
		}

		public void AddSprite(FPTexture texture, float X, float Y, float Rotation)
		{
			float w = texture.Size.Width;
			float h = texture.Size.Height;
			_tiles.Add(texture.Tile);
			_transforms.Add(SKRotationScaleMatrix.Create(1.0f, Rotation * MathF.PI / 180.0f, X + w / 2.0f, Y + h / 2.0f, w / 2.0f, h / 2.0f));
			_colors.Add(new SKColor(255, 255, 255, 255));
		}

		public void AddSprite(FPTexture texture, float X, float Y, SKColor color)
		{
			float w = texture.Size.Width;
			float h = texture.Size.Height;
			_tiles.Add(texture.Tile);
			_transforms.Add(SKRotationScaleMatrix.CreateTranslation(X, Y));
			_colors.Add(color);
		}

		public void DrawAll()
		{
			if (_atlasImage == null)
			{
				using var stream = AssetLoader.Open(new Uri($"avares://IronJumpAvalonia/Assets/levelatlas.png"));
				_atlasImage = SKImage.FromEncodedData(stream);
			}

			_context.Custom(new AtlasDrawOperation(_atlasImage, _bounds, _tiles.ToArray(), _transforms.ToArray(), _colors.ToArray()));
		}
	}
	
}
