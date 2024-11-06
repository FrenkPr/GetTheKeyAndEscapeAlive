using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TopDownGame
{
    class SettingsMenuScene : MenuScene
    {
        private ComboBoxOption<TextObject, string> resizeWindowOption;
        private ComboBoxOption<TextObject, bool> fullScreenOption;
        private ComboBoxOption<TextObject, float> toggleMusicVolumeOption;
        private TextObject applySettings;
        private TextObject settingsTextObject;

        private XmlNode currentWindowSizeNode;
        private XmlNode currentFullscreenValueNode;
        private XmlNode currentMusicVolumeValueNode;

        public SettingsMenuScene()
        {

        }

        public override void Start()
        {
            base.Start();

            TextObject textObject;

            Vector2 menuPos = new Vector2(0.5f, 1.5f);
            PreviousMenu = new MainMenuScene();

            //title menu text (header)
            settingsTextObject = new TextObject("SETTINGS", new Vector2(Game.OrthoHalfWidth, 0.1f), DrawingType.PauseMenu, UpdatingType.PauseMenu);
            settingsTextObject.EditPosition(new Vector2(settingsTextObject.Position.X - settingsTextObject.GetTextWidth() * 0.5f, settingsTextObject.Position.Y));

            AddMenuOption(settingsTextObject, null, false, ref menuPos, false);



            //menu body

            //WINDOW RESIZE OPTION
            ComboBoxOption<TextObject, string>.AddEditableOption(out resizeWindowOption, out currentWindowSizeNode, "windowResizeValues", "windowSize", "stringValue", "stringValue", "CHANGE WINDOW SIZE ", ref menuPos);

            //CHANGE FULLSCREEN OPTION
            ComboBoxOption<TextObject, bool>.AddEditableOption(out fullScreenOption, out currentFullscreenValueNode, "windowFullScreenValues", "fullScreen", "stringValue", "boolValue", "FULLSCREEN ", ref menuPos);

            //TOGGLE MUSIC VOLUME OPTION
            ComboBoxOption<TextObject, float>.AddEditableOption(out toggleMusicVolumeOption, out currentMusicVolumeValueNode, "musicVolumeValues", "musicVolume", "stringValue", "volumeValue", "MUSIC VOLUME ", ref menuPos);



            //menu footer
            textObject = new TextObject(">Back", new Vector2(0.5f, Game.OrthoHeight - 1), DrawingType.PauseMenu, UpdatingType.PauseMenu, isClickable: true);
            AddMenuOption(textObject, new MainMenuScene(), true, ref menuPos, false);

            applySettings = new TextObject("Apply", new Vector2(Game.OrthoWidth - 3.5f, Game.OrthoHeight - 1), DrawingType.PauseMenu, UpdatingType.PauseMenu, isClickable: true);
            applySettings.InitGUICamera();
            applySettings.IsActive = false;


            InitMenuOptionsGUICamera();
        }

        public override void Update()
        {
            base.Update();

            if (HasChangedEditableOptionValue && !applySettings.IsActive)
            {
                applySettings.IsActive = true;
            }

            if (applySettings.OnClick())
            {
                string[] newWindowSize = resizeWindowOption.CurrentOptionValue.Split('x');

                //updates fullscreen settings
                if (fullScreenOption.CurrentOptionValue)  //if we gotta set the window to fullscreen
                {
                    Game.Window.Position = Game.StartWindowPosition;
                    Game.Window.SetFullScreen(fullScreenOption.CurrentOptionValue);
                    Game.Window.SetResolution(int.Parse(newWindowSize[0]), int.Parse(newWindowSize[1]));
                }

                //updates window size settings
                else
                {
                    Game.Window.Position = Vector2.Zero;
                    Game.Window.SetFullScreen(fullScreenOption.CurrentOptionValue);
                    Game.Window.SetResolution(1920, 1080);
                }

                Game.Window.SetSize(int.Parse(newWindowSize[0]), int.Parse(newWindowSize[1]));

                CameraMngr.OnChangeWindowSize();
                OnChangeWindowSizeMngr.OnChangeWindowSize();

                //updates music volume settings
                SoundEmitter.SetAudioVolume(toggleMusicVolumeOption.CurrentOptionValue);

                //UPDATING XML GAME CONFIG ATTRIBUTES

                //"windowSize" node
                currentWindowSizeNode.Attributes[0].Value = newWindowSize[0];
                currentWindowSizeNode.Attributes[1].Value = newWindowSize[1];
                currentWindowSizeNode.Attributes[2].Value = resizeWindowOption.CurrentOptionToDisplay.Text;

                //"fullscreen" node
                currentFullscreenValueNode.Attributes[0].Value = fullScreenOption.CurrentOptionValue.ToString();
                currentFullscreenValueNode.Attributes[1].Value = fullScreenOption.CurrentOptionToDisplay.Text;

                //"musicVolume" node
                currentMusicVolumeValueNode.Attributes[0].Value = toggleMusicVolumeOption.CurrentOptionValue.ToString();
                currentMusicVolumeValueNode.Attributes[1].Value = toggleMusicVolumeOption.CurrentOptionToDisplay.Text;

                Game.XmlGameConfigDoc.Save("Assets/CONFIG/GameConfig.xml");

                HasChangedEditableOptionValue = false;
                applySettings.IsActive = false;
                //TextObject.TextObjectClicked = null;

                settingsTextObject.EditPosition(new Vector2(Game.OrthoHalfWidth - settingsTextObject.GetTextWidth() * 0.5f, 0.1f));
                applySettings.EditPosition(new Vector2(Game.OrthoWidth - 3.5f, Game.OrthoHeight - 1));
            }
        }

        public override void OnExit()
        {
            menuOptions = null;

            UpdateMngr.Clear(UpdatingType.PauseMenu);
            DrawMngr.Clear(DrawingType.PauseMenu);
        }
    }
}
