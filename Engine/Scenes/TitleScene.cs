using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownGame
{
    class TitleScene : Scene
    {
        TextObject[] texts;

        public TitleScene() : base()
        {

        }

        protected override void LoadAssets()
        {
            FontMngr.AddFont("comics", "comics", "Assets/FONTS/comics.png", 10, 10, 32);
        }

        public override void Start()
        {
            LoadAssets();

            texts = new TextObject[2];

            texts[0] = new TextObject("   Get The Key\nAnd Escape Alive", new Vector2(Game.OrthoHalfWidth, 4));
            texts[1] = new TextObject("PRESS ENTER TO START", new Vector2(Game.OrthoHalfWidth, 6));

            texts[0].EditPosition(new Vector2(texts[0].Position.X - texts[0].GetTextWidth() * 0.5f, texts[0].Position.Y));
            texts[1].EditPosition(new Vector2(texts[1].Position.X - texts[1].GetTextWidth() * 0.5f, texts[1].Position.Y));

            base.Start();
        }

        public override void Update()
        {
            if (Game.KeyboardCtrl.OnKeyPressed(KeyCodeType.Confirm))
            {
                IsPlaying = false;
            }

            DrawMngr.Draw(DrawingType.Play);
        }

        public override void OnExit()
        {
            DrawMngr.ClearAll();
            UpdateMngr.ClearAll();
            TextureMngr.ClearAll();

            texts = null;

            FontMngr.ClearAll();

            NextScene = new ChooseCharacterScene();
            //NextScene = new PlayerHomeScene(52, 9);
            //NextScene = new KeyScene(3, 17);
            //NextScene = new FinalScene(30, 17);
        }
    }
}
