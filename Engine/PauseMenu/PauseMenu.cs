using Aiv.Fast2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownGame
{
    class PauseMenu
    {
        public MenuScene CurrentMenuScene { get; private set; }
        public bool IsActive;
        private bool isPauseMenuValuePressed;
        public bool PlayerInputsLocked;

        public PauseMenu()
        {
            CurrentMenuScene = new MainMenuScene();
        }

        public void Input()
        {
            TogglePauseMenu();
        }

        private void TogglePauseMenu()
        {
            if (Game.KeyboardCtrl.OnKeyPressed(KeyCodeType.TogglePauseMenu))
            {
                if (!isPauseMenuValuePressed)
                {
                    if (!IsActive)
                    {
                        IsActive = true;
                        CurrentMenuScene.Start();
                    }

                    else if (CurrentMenuScene.PreviousMenu != null)
                    {
                        CurrentMenuScene.OnExit();

                        CurrentMenuScene = CurrentMenuScene.PreviousMenu;
                        CurrentMenuScene.Start();
                    }

                    else
                    {
                        CurrentMenuScene.IsPlaying = false;
                    }

                    isPauseMenuValuePressed = true;
                }
            }

            else if (isPauseMenuValuePressed)
            {
                isPauseMenuValuePressed = false;
            }
        }

        public void Update()
        {
            if (!IsActive)
            {
                return;
            }

            if (CurrentMenuScene.IsPlaying)
            {
                CurrentMenuScene.Update();
            }

            else
            {
                CurrentMenuScene.OnExit();

                if (CurrentMenuScene.NextMenuScene != null)
                {
                    CurrentMenuScene = CurrentMenuScene.NextMenuScene;
                    CurrentMenuScene.Start();
                }
            }
        }
    }
}
