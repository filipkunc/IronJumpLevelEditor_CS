using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using IronJumpLevelEditor_CS.PortedClasses;

namespace IronJumpLevelEditor_CS
{
    public class GameObjectFactory
    {
        public Bitmap Image { get; private set; }
        public Func<FPGameObject> FactoryAction { get; private set; }

        public GameObjectFactory(Bitmap image, Func<FPGameObject> factoryAction)
        {
            Image = image;
            FactoryAction = factoryAction;
        }
    }
}
