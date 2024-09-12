using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownGame
{
    static class OnChangeWindowSizeMngr
    {
        private static List<IOnChangeWindowSize> objs;

        static OnChangeWindowSizeMngr()
        {
            objs = new List<IOnChangeWindowSize>();
        }

        public static void Add(IOnChangeWindowSize item)
        {
            if (!objs.Contains(item))
            {
                objs.Add(item);
            }
        }

        public static void Remove(IOnChangeWindowSize item)
        {
            if (objs.Contains(item))
            {
                objs.Remove(item);
            }
        }

        public static void ClearAll()
        {
            objs.Clear();
        }

        public static void OnChangeWindowSize()
        {
            for (int i = 0; i < objs.Count; i++)
            {
                objs[i].OnChangeWindowSize();
            }
        }
    }
}
