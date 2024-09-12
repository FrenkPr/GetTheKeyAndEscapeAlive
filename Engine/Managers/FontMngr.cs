using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownGame
{
    static class FontMngr
    {
        private static Dictionary<string, Font> fonts;

        static FontMngr()
        {
            fonts = new Dictionary<string, Font>();
        }

        public static Font AddFont(string fontId, string textureId, string texturePath, int numCols, int numRows, int firstASCIIvalue)
        {
            Font f = new Font(textureId, texturePath, numCols, numRows, firstASCIIvalue);
            fonts.Add(fontId, f);

            return f;
        }

        public static Font GetFont(string fontId)
        {
            return fonts[fontId];
        }

        public static void ClearAll()
        {
            fonts.Clear();
        }
    }
}
