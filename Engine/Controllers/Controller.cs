using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownGame
{
    abstract class Controller
    {
        protected int controllerIndex;

        public Controller(int index)
        {
            controllerIndex = index;
        }
    }
}
