using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiv.Fast2D;
using System.IO;
using OpenTK;

namespace TopDownGame
{
    class GameOverScene : Scene
    {
        private KeyCode keyValuePressed;
        private TextObject gameOverTextObject;
        private Scene previousScene;

        public GameOverScene(Scene previousScene) : base()
        {
            this.previousScene = previousScene;
        }

        protected override void LoadAssets()
        {
            FontMngr.AddFont("comics", "comics", "Assets/FONTS/comics.png", 10, 10, 32);
        }

        public override void Start()
        {
            LoadAssets();

            gameOverTextObject = new TextObject("         GAME OVER\n\nPRESS ENTER TO PLAY AGAIN.\n    PRESS ESC TO QUIT.", Game.ScreenCenter - new Vector2(0, 1), charWidth: 50, charHeight: 50);
            gameOverTextObject.EditPosition(new Vector2(gameOverTextObject.Position.X - gameOverTextObject.GetTextWidth() * 0.5f, gameOverTextObject.Position.Y));
            gameOverTextObject.InitGUICamera();

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

                NextScene = previousScene;
            }
        }
    }
}
