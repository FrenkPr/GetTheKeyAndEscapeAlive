using OpenTK;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownGame
{
    abstract class GUI_Item : GameObject
    {
        public GUI_Item(string textureId, DrawingType dType, UpdatingType uType, float width = 0, float height = 0, DrawLayer dLayer = DrawLayer.GUI, Vector4? color = null) : base(textureId, 1, width, height, dLayer, dType, uType, color)
        {
            if (CameraMngr.GetCamera("GUI") != null)
            {
                Sprite.Camera = CameraMngr.GetCamera("GUI");
            }
            
            IsActive = true;
        }

        public void SetSpriteColor(Vector4 color)
        {
            Sprite.SetMultiplyTint(color);
        }
    }
}
