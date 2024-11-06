using Aiv.Fast2D;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TopDownGame
{
    class ChooseCharacterScene : Scene
    {
        private static ComboBoxOption<GameObject, string> choosePlayerOption;
        private XmlNode playerStartNode;
        private XmlNode playerChosenNode;
        private TextObject confirmCharacterTextObject;
        private DivBox confirmCharacterDiv;

        public ChooseCharacterScene() : base()
        {

        }

        protected override void LoadAssets()
        {
            FontMngr.AddFont("comics", "comics", "Assets/FONTS/comics.png", 10, 10, 32);

            string[] actorNames = new string[3];

            actorNames[0] = "Adventurer";
            actorNames[1] = "Dog";
            actorNames[2] = "Princess";

            for (int i = 0; i < actorNames.Length; i++)
            {
                TextureMngr.AddTexture(actorNames[i], $"Assets/SPRITES/HEROS/spritesheets/{actorNames[i]}/HEROS8bit_{actorNames[i]} Idle D.png");
            }

            TextureMngr.AddTexture("frame", "Assets/Images/frame.jpg");
        }

        public override void Start()
        {
            LoadAssets();

            Vector2 pos = new Vector2(0, Game.ScreenCenter.Y);
            bool initGUICamera = CameraMngr.MainCamera == null ? false : true;

            ComboBoxOption<GameObject, string>.AddEditableOption(out choosePlayerOption, out playerStartNode, "playerTypes", "playerType1", "name", "name", "CHOOSE A CHARACTER", ref pos, initGUICamera, DrawingType.Play, UpdatingType.Play);

            choosePlayerOption.OptionInfo.EditPosition(new Vector2(Game.ScreenCenter.X - choosePlayerOption.OptionInfo.GetTextWidth() * 0.5f, Game.ScreenCenter.Y - choosePlayerOption.CurrentOptionToDisplayWidth * 0.5f - 1));
            choosePlayerOption.OptionsToDisplayPosition = Game.ScreenCenter;
            choosePlayerOption.NextOption.EditPosition(new Vector2(choosePlayerOption.OptionsToDisplayPosition.X + choosePlayerOption.CurrentOptionToDisplayWidth + 0.2f, choosePlayerOption.OptionsToDisplayPosition.Y + choosePlayerOption.CurrentOptionToDisplayHeight * 0.5f - choosePlayerOption.NextOption.GetTextHeight() * 0.5f));
            choosePlayerOption.PreviousOption.EditPosition(new Vector2(choosePlayerOption.OptionsToDisplayPosition.X - choosePlayerOption.CurrentOptionToDisplayWidth - 0.2f, choosePlayerOption.OptionsToDisplayPosition.Y + choosePlayerOption.CurrentOptionToDisplayHeight * 0.5f - choosePlayerOption.PreviousOption.GetTextHeight() * 0.5f));
            choosePlayerOption.AlignCurrentOptionPosition();

            confirmCharacterTextObject = new TextObject("Confirm", new Vector2(Game.ScreenCenter.X, Game.ScreenCenter.Y + 2), isClickable: true);
            confirmCharacterTextObject.EditPosition(new Vector2(confirmCharacterTextObject.Position.X - confirmCharacterTextObject.GetTextWidth() * 0.5f + 0.1f, confirmCharacterTextObject.Position.Y));

            confirmCharacterDiv = new DivBox("frame", DrawingType.Play, UpdatingType.Play, DrawLayer.Playground, 500, 150);
            confirmCharacterDiv.Sprite.pivot = Vector2.Zero;
            confirmCharacterDiv.Position = confirmCharacterTextObject.Position;
            confirmCharacterDiv.X -= 0.3f;
            confirmCharacterDiv.Y -= 0.4f;
            confirmCharacterDiv.SetSpriteColor(new Vector4(1, 0, 0, 1));

            if (initGUICamera)
            {
                confirmCharacterTextObject.InitGUICamera();
            }

            base.Start();
        }

        public override void Update()
        {
            if (confirmCharacterTextObject.OnClick() ||
                (Game.KeyboardCtrl.OnKeyPressed(KeyCodeType.Confirm) && KeyboardController.LastKeyReleased == Game.KeyboardCtrl.Keys.KeyCode[KeyCodeType.Confirm]))
            {
                playerChosenNode = Game.XmlGameConfigDoc.GetElementsByTagName("currentPlayerType")[0];
                playerChosenNode.Attributes[0].Value = choosePlayerOption.CurrentOptionValue;
                Game.XmlGameConfigDoc.Save("Assets/CONFIG/GameConfig.xml");

                IsPlaying = false;
            }

            UpdateMngr.Update(UpdatingType.Play);
            DrawMngr.Draw(DrawingType.Play);
        }

        public override void OnExit()
        {
            DrawMngr.ClearAll();
            UpdateMngr.ClearAll();
            TextureMngr.ClearAll();
            FontMngr.ClearAll();

            choosePlayerOption = null;
            confirmCharacterTextObject = null;
            playerStartNode = null;
            playerChosenNode = null;

            NextScene = new PlayerHomeScene(52, 9);
        }
    }
}
