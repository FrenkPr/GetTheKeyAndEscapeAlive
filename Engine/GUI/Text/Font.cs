using Aiv.Fast2D;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownGame
{
    class Font
    {
        public Texture FontTexture { get; }
        public string FontTextureID { get; }
        public int CharWidth { get; }
        public int CharHeight { get; }
        private int firstCharASCIIvalue;
        private int numCols;

        public Font(string textureId, string texturePath, int numCols, int numRows, int firstASCIIvalue)
        {
            FontTexture = TextureMngr.AddTexture(textureId, texturePath);
            FontTextureID = textureId;

            CharWidth = FontTexture.Width / numCols;
            CharHeight = FontTexture.Height / numRows;
            firstCharASCIIvalue = firstASCIIvalue;

            this.numCols = numCols;
        }

        public Vector2 GetCharOffset(char c)
        {
            int ASCIIvalue = c;

            int delta = ASCIIvalue - firstCharASCIIvalue;
            int x = delta % numCols;
            int y = delta / numCols;

            return new Vector2(x * CharWidth, y * CharHeight);
        }
    }
}
