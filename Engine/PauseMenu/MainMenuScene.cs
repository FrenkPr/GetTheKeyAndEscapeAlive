using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownGame
{
    class MainMenuScene : MenuScene
    {
        private TextObject quitText;

        public MainMenuScene()
        {

        }

        public override void Start()
        {
            base.Start();

            TextObject pauseMenuText = new TextObject("PAUSE MENU", new Vector2(Game.OrthoHalfWidth, 0.1f), dType: DrawingType.PauseMenu, uType: UpdatingType.PauseMenu);
            pauseMenuText.EditPosition(new Vector2(pauseMenuText.Position.X - pauseMenuText.GetTextWidth() * 0.5f, pauseMenuText.Position.Y));

            Vector2 menuPos = new Vector2(1);
            Vector2 pos = pauseMenuText.Position;

            AddMenuOption(pauseMenuText, null, false, ref pos, false);
            AddMenuOption(new TextObject("RESUME", new Vector2(1), dType: DrawingType.PauseMenu, uType: UpdatingType.PauseMenu, isClickable: true), null, true, ref menuPos);
            AddMenuOption(new TextObject("SETTINGS", new Vector2(1, menuPos.Y), dType: DrawingType.PauseMenu, uType: UpdatingType.PauseMenu, isClickable: true), new SettingsMenuScene(), true, ref menuPos);

            quitText = new TextObject("QUIT", new Vector2(1, menuPos.Y), dType: DrawingType.PauseMenu, uType: UpdatingType.PauseMenu, isClickable: true);
            AddMenuOption(quitText, null, true, ref menuPos);

            InitMenuOptionsGUICamera();
        }

        public override void OnExit()
        {
            menuOptions = null;

            if (TextObject.LastTextObjectClicked == quitText)
            {
                Game.CurrentScene.IsPlaying = false;
                Game.CurrentScene.NextScene = null;
            }

            if (NextMenuScene == null)
            {
                Game.CurrentScene.PauseMenu.PlayerInputsLocked = true;
                Game.CurrentScene.PauseMenu.IsActive = false;
            }

            UpdateMngr.Clear(UpdatingType.PauseMenu);
            DrawMngr.Clear(DrawingType.PauseMenu);
        }
    }
}
