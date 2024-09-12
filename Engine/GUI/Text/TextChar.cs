using Aiv.Fast2D;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownGame
{
    class TextChar : GameObject
    {
        private Font font;
        public char Char;

        public TextChar(Font font, char textChar, Vector2 pos, DrawingType dType, UpdatingType uType, float charWidth, float charHeight) : base(font.FontTextureID, 1, charWidth, charHeight, DrawLayer.GUI, dType, uType)
        {
            this.font = font;
            Position = pos;
            Char = textChar;
            Sprite.pivot = Vector2.Zero;

            UpdateMngr.Remove(this, uType);
        }

        public override void Draw()
        {
            if (IsActive)
            {
                Vector2 offset = font.GetCharOffset(Char);

                Sprite.DrawTexture(font.FontTexture, (int)offset.X, (int)offset.Y, font.CharWidth, font.CharHeight);
            }
        }
    }
}
