using Aiv.Fast2D;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownGame
{
    class EndingScene : Scene
    {
        private KeyCode keyValuePressed;
        private TextObject endingTextObjects;

        public EndingScene() : base()
        {

        }

        protected override void LoadAssets()
        {
            FontMngr.AddFont("comics", "comics", "Assets/FONTS/comics.png", 10, 10, 32);
        }

        public override void Start()
        {
            LoadAssets();

            endingTextObjects = new TextObject("     CONGRATULATIONS!\n YOU COMPLETED THE GAME.\n\nPRESS ENTER TO PLAY AGAIN.\n    PRESS ESC TO QUIT.", Game.ScreenCenter - new Vector2(0, 1), charWidth: 50, charHeight: 50);
            endingTextObjects.EditPosition(new Vector2(endingTextObjects.Position.X - endingTextObjects.GetTextWidth() * 0.5f, endingTextObjects.Position.Y));
            endingTextObjects.InitGUICamera();

            //endingTextObjects = new TextObject[3];

            //endingTextObjects[0] = new TextObject("CONGRATULATIONS!", Game.ScreenCenter - new Vector2(0, 0.5f), charWidth: 50, charHeight: 50);
            //endingTextObjects[0].EditPosition(new Vector2(endingTextObjects[0].Position.X - endingTextObjects[0].GetTextWidth() * 0.5f, endingTextObjects[0].Position.Y));

            //endingTextObjects[1] = new TextObject("YOU COMPLETED THE GAME.", Game.ScreenCenter + new Vector2(0, 0.5f), charWidth: 50, charHeight: 50);
            //endingTextObjects[1].EditPosition(new Vector2(endingTextObjects[1].Position.X - endingTextObjects[1].GetTextWidth() * 0.5f, endingTextObjects[1].Position.Y));

            //endingTextObjects[2] = new TextObject("PRESS ENTER TO PLAY AGAIN.\n    PRESS ESC TO QUIT.", Game.ScreenCenter + new Vector2(0, 1.5f), charWidth: 50, charHeight: 50);
            //endingTextObjects[2].EditPosition(new Vector2(endingTextObjects[2].Position.X - endingTextObjects[2].GetTextWidth() * 0.5f, endingTextObjects[2].Position.Y));

            base.Start();
        }

        public override void Update()
        {
            if (Game.KeyboardCtrl.OnKeyPressed(KeyCodeType.Confirm) || Game.KeyboardCtrl.OnKeyPressed(KeyCodeType.GoBack))
            {
                if (Game.KeyboardCtrl.OnKeyPressed(KeyCodeType.Confirm))
                {
                    keyValuePressed = KeyCode.Return;
                }

                else
                {
                    keyValuePressed = KeyCode.Esc;
                }

                IsPlaying = false;
            }

            DrawMngr.Draw(DrawingType.Play);
        }

        public override void OnExit()
        {
            if (keyValuePressed == KeyCode.Esc)
            {
                NextScene = null;
            }

            else
            {
                DrawMngr.ClearAll();
                TextureMngr.ClearAll();
                FontMngr.ClearAll();
                Game.RestoreXmlAttributeValues();

                NextScene = new ChooseCharacterScene();
            }
        }
    }
}
