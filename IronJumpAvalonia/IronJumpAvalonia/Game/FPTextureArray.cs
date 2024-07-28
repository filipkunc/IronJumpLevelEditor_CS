using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJumpAvalonia.Game
{
    public class FPTextureArray
    {
        List<FPTexture> textures = new List<FPTexture>();

        public FPTextureArray()
        {

        }

        public void AddTexture(FPTexture texture)
        {
            textures.Add(texture);
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
