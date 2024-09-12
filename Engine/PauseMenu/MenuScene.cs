using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownGame
{
    abstract class MenuScene
    {
        public bool IsPlaying;
        public MenuScene NextMenuScene;
        public MenuScene PreviousMenu;

        //the MenuScene value represents the next menu to be set to the curretMenuScene variable on PauseMenu class,
        //while the bool value specifies, when option clicked, if must go to the next menu scene or not.
        protected Dictionary<TextObject, Tuple<MenuScene, bool>> menuOptions;

        public bool HasChangedEditableOptionValue;

        public MenuScene()
        {

        }

        public virtual void Start()
        {
            IsPlaying = true;
            menuOptions = new Dictionary<TextObject, Tuple<MenuScene, bool>>();
            HasChangedEditableOptionValue = false;
        }

        protected void AddMenuOption(TextObject menuText, MenuScene nextMenuScene, bool goToNextMenuScene, ref Vector2 menuPos, bool incmenuPosY = true)
        {
            menuOptions.Add(menuText, new Tuple<MenuScene, bool>(nextMenuScene, goToNextMenuScene));

            if (incmenuPosY)
            {
                menuPos.Y += 1;
            }
        }

        protected void InitMenuOptionsGUICamera()
        {
            foreach (TextObject textObject in menuOptions.Keys)
            {
                textObject.InitGUICamera();
            }
        }

        protected void OnMenuOptionClicked()
        {
            //if (TextObject.TextObjectClicked != null && menuOptions.ContainsKey(TextObject.TextObjectClicked))
            //{
            //    NextMenuScene = menuOptions[TextObject.TextObjectClicked].Item1;
            //    IsPlaying = !menuOptions[TextObject.TextObjectClicked].Item2;
            //}

            foreach (TextObject menuOptionText in menuOptions.Keys)
            {
                if (menuOptionText.OnClick())
                {
                    NextMenuScene = menuOptions[menuOptionText].Item1;
                    IsPlaying = !menuOptions[menuOptionText].Item2;
                }
            }
        }

        public virtual void Update()
        {
            OnMenuOptionClicked();
        }

        public abstract void OnExit();
    }
}
