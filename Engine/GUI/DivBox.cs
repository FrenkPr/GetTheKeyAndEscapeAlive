using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownGame
{
    class DivBox : GUI_Item
    {
        public DivBox(string textureId, DrawingType dType, UpdatingType uType, DrawLayer dLayer = DrawLayer.Foreground, float width = 0, float height = 0, Vector4? color = null) : base(textureId, dType, uType, width, height, dLayer, color)
        {

        }
    }
}
