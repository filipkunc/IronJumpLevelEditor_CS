using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GLCanvas;
using System.Drawing;

namespace IronJumpLevelEditor_CS.PortedClasses
{
    public class FPTextureArray
    {
        List<FPTexture> textures = new List<FPTexture>();

        public FPTextureArray()
        {

        }

        public void AddTexture(FPCanvas canvas, Bitmap bitmap)
        {
            textures.Add(canvas.CreateTexture(bitmap));        
        }

        public FPTexture this[int index]
        {
            get { return textures[index]; }
        }

        public int Count
        {
            get { return textures.Count; }
        }
    }
}
