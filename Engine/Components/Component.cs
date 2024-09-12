using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownGame
{
    class Component
    {
        public GameObject GameObject { get; protected set; }
        private bool isEnabled;
        public bool IsEnabled { get { return isEnabled && GameObject.IsActive; } set { isEnabled = value; GameObject.IsActive = value; } }

        public Component(GameObject owner)
        {
            GameObject = owner;
        }
    }
}
