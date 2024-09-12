using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownGame
{
    enum MouseButton
    {
        LeftClick,
        RightClick
    }

    static class MouseController
    {
        public static bool IsMouseButtonPressed(MouseButton value)
        {
            bool res = false;

            switch (value)
            {
                case MouseButton.LeftClick:
                    res = Game.Window.MouseLeft;
                    break;

                case MouseButton.RightClick:
                    res = Game.Window.MouseRight;
                    break;
            }

            return res;
        }
    }
}
