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
    class ComboBoxOption<T, R> : IUpdatable
    {
        public TextObject OptionInfo { get; }
        public TextObject NextOption { get; }  //when clicked, CurrentOption will go to next option
        public TextObject PreviousOption { get; }  //when clicked, CurrentOption will go to previous option

        public T CurrentOptionToDisplay { get; private set; }

        //options to display only works with TextObject and GameObject types
        //to let it work with other objects, add the CloneList method to that object type
        //the CloneList method will allow to copy a List as value type
        public readonly List<T> OptionsToDisplay;

        public R CurrentOptionValue { get; private set; }
        private List<R> optionsValues;

        private int currentOptionIndex;

        private UpdatingType updatingType;

        public Vector2 OptionsToDisplayPosition
        {
            get
            {
                if (CurrentOptionToDisplay is GameObject currentOption)
                {
                    return currentOption.Position;
                }

                else if (CurrentOptionToDisplay is TextObject _currentOption)
                {
                    return _currentOption.Position;
                }

                return Vector2.Zero;
            }

            set
            {
                if (CurrentOptionToDisplay is GameObject currentOption)
                {
                    currentOption.Position = value;

                    foreach (T option in OptionsToDisplay)
                    {
                        if (option is GameObject obj)
                        {
                            obj.Position = value;
                        }
                    }
                }

                else if (CurrentOptionToDisplay is TextObject _currentOption)
                {
                    _currentOption.EditPosition(value);

                    foreach (T option in OptionsToDisplay)
                    {
                        if (option is TextObject obj)
                        {
                            obj.EditPosition(value);
                        }
                    }
                }
            }
        }

        public float CurrentOptionToDisplayWidth
        {
            get
            {
                if (CurrentOptionToDisplay is GameObject currentOption)
                {
                    return currentOption.Width;
                }

                else if (CurrentOptionToDisplay is TextObject _currentOption)
                {
                    return _currentOption.GetTextWidth();
                }

                return 0;
            }
        }

        public float CurrentOptionToDisplayHeight
        {
            get
            {
                if (CurrentOptionToDisplay is GameObject currentOption)
                {
                    return currentOption.Height;
                }

                else if (CurrentOptionToDisplay is TextObject _currentOption)
                {
                    return _currentOption.GetTextHeight();
                }

                return 0;
            }
        }

        public bool IsActive
        {
            get
            {
                if (CurrentOptionToDisplay is GameObject currentOption)
                {
                    return currentOption.IsActive;
                }

                else if (CurrentOptionToDisplay is TextObject _currentOption)
                {
                    return _currentOption.IsActive;
                }

                return false;
            }

            set
            {
                if (CurrentOptionToDisplay is GameObject currentOption)
                {
                    //sets all options values to display IsActive to false
                    foreach (T option in OptionsToDisplay)
                    {
                        if (option is GameObject obj)
                        {
                            obj.IsActive = false;
                        }
                    }

                    currentOption.IsActive = value;
                }

                else if (CurrentOptionToDisplay is TextObject _currentOption)
                {
                    //sets all options values to display IsActive to false
                    foreach (T option in OptionsToDisplay)
                    {
                        if (option is TextObject obj)
                        {
                            obj.IsActive = false;
                        }
                    }

                    _currentOption.IsActive = value;
                }
            }
        }

        public ComboBoxOption(TextObject optionTextObjectInfo, TextObject nextTextObject, TextObject prevTextObject, List<T> optionsToDisplay, List<R> optionsValues, int startOptionIndex, Vector2 pos, UpdatingType uType = UpdatingType.PauseMenu)
        {
            OptionInfo = optionTextObjectInfo;
            NextOption = nextTextObject;
            PreviousOption = prevTextObject;

            if (optionsToDisplay[0] is TextObject)
            {
                OptionsToDisplay = (List<T>)((IList<TextObject>)optionsToDisplay).CloneList();
            }

            else if (optionsToDisplay[0] is GameObject)
            {
                OptionsToDisplay = (List<T>)((IList<GameObject>)optionsToDisplay).CloneList();
            }

            this.optionsValues = optionsValues;

            currentOptionIndex = startOptionIndex;

            updatingType = uType;

            CurrentOptionToDisplay = OptionsToDisplay[currentOptionIndex];
            CurrentOptionValue = optionsValues[currentOptionIndex];

            OptionsToDisplayPosition = pos;

            UpdateMngr.Add(this, updatingType);
        }

        public void InitOptionsToDisplayGUICamera()
        {
            OptionInfo.InitGUICamera();
            NextOption.InitGUICamera();
            PreviousOption.InitGUICamera();

            foreach (object optionToDisplay in OptionsToDisplay)
            {
                if (optionToDisplay is TextObject option)
                {
                    option.InitGUICamera();
                }

                else if (optionToDisplay is GameObject gameObj)
                {
                    gameObj.Sprite.Camera = CameraMngr.GetCamera("GUI");
                }
            }

        }

        private void Input()
        {
            if (NextOption.OnClick())
            {
                NextOptionToDisplay();
            }

            if (PreviousOption.OnClick())
            {
                PreviousOptionToDisplay();
            }
        }

        private void NextOptionToDisplay()
        {
            if (currentOptionIndex + 1 == OptionsToDisplay.Count)
            {
                return;
            }

            currentOptionIndex++;
            CurrentOptionToDisplay = OptionsToDisplay[currentOptionIndex];
            CurrentOptionValue = optionsValues[currentOptionIndex];

            IsActive = true;

            AlignCurrentOptionPosition();

            if (updatingType == UpdatingType.PauseMenu && Game.CurrentScene.PauseMenu != null)
            {
                Game.CurrentScene.PauseMenu.CurrentMenuScene.HasChangedEditableOptionValue = true;
            }

            //Console.WriteLine("Switched");
        }

        private void PreviousOptionToDisplay()
        {
            if (currentOptionIndex - 1 < 0)
            {
                return;
            }

            currentOptionIndex--;
            CurrentOptionToDisplay = OptionsToDisplay[currentOptionIndex];
            CurrentOptionValue = optionsValues[currentOptionIndex];

            IsActive = true;

            AlignCurrentOptionPosition();

            if (updatingType == UpdatingType.PauseMenu && Game.CurrentScene.PauseMenu != null)
            {
                Game.CurrentScene.PauseMenu.CurrentMenuScene.HasChangedEditableOptionValue = true;
            }
        }

        public void AlignCurrentOptionPosition()
        {
            Vector2 pos = NextOption.Position - PreviousOption.Position;
            pos.X += PreviousOption.GetTextWidth();

            OptionsToDisplayPosition = new Vector2(PreviousOption.Position.X + pos.X * 0.5f - CurrentOptionToDisplayWidth * 0.5f, OptionsToDisplayPosition.Y);
        }

        public static void AddEditableOption(out ComboBoxOption<T, R> editableOption, out XmlNode currentOptionNode, string optionsToDisplayTagName, string currentOptionTagName, string optionsToDisplayAttributeName, string optionValuesAttributeName, string optionTextInfo, ref Vector2 menuPos, bool initGUICamera = true, DrawingType dType = DrawingType.PauseMenu, UpdatingType uType = UpdatingType.PauseMenu)
        {
            TextObject optionTextObjectInfo;

            TextObject nextOption;
            TextObject previousOption;

            int startOptionIndex = 0;
            List<T> editableOptionsToDisplay = new List<T>();
            List<T> widestOptionToDisplay = new List<T>();

            XmlNode xmlNode;
            XmlNodeList xmlNodeList;

            optionTextObjectInfo = new TextObject(optionTextInfo, menuPos, dType, uType, 40, 40);

            xmlNode = Game.XmlGameConfigDoc.GetElementsByTagName(optionsToDisplayTagName)[0];
            xmlNodeList = xmlNode.ChildNodes;
            currentOptionNode = Game.XmlGameConfigDoc.GetElementsByTagName(currentOptionTagName)[0];

            List<R> optionValues = new List<R>();

            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                //options to display init
                if (editableOptionsToDisplay is List<TextObject> optionsToDisplay)
                {
                    optionsToDisplay.Add(new TextObject(XmlUtilities.GetStringAttribute(xmlNodeList[i], optionsToDisplayAttributeName), Vector2.Zero, dType, uType, 40, 40));
                }

                else if (editableOptionsToDisplay is List<GameObject> _optionsToDisplay)
                {
                    GameObject optionToDisplay = new GameObject(XmlUtilities.GetStringAttribute(xmlNodeList[i], optionsToDisplayAttributeName), 1, 64, 64, DrawLayer.GUI, dType, uType);
                    optionToDisplay.Sprite.pivot = Vector2.Zero;

                    _optionsToDisplay.Add(optionToDisplay);
                }
                //end options to display init

                //option values init
                if (optionValues is List<string> stringOptions)
                {
                    stringOptions.Add(XmlUtilities.GetStringAttribute(xmlNodeList[i], optionValuesAttributeName));

                    if (stringOptions[i] == XmlUtilities.GetStringAttribute(currentOptionNode, optionValuesAttributeName))
                    {
                        startOptionIndex = i;
                    }
                }

                else if (optionValues is List<bool> boolOptions)
                {
                    boolOptions.Add(XmlUtilities.GetBoolAttribute(xmlNodeList[i], optionValuesAttributeName));

                    if (boolOptions[i] == XmlUtilities.GetBoolAttribute(currentOptionNode, optionValuesAttributeName))
                    {
                        startOptionIndex = i;
                    }
                }

                else if (optionValues is List<float> floatOptions)
                {
                    floatOptions.Add(XmlUtilities.GetFloatAttribute(xmlNodeList[i], optionValuesAttributeName));

                    if (floatOptions[i] == XmlUtilities.GetFloatAttribute(currentOptionNode, optionValuesAttributeName))
                    {
                        startOptionIndex = i;
                    }
                }

                else if (optionValues is List<float> intOptions)
                {
                    intOptions.Add(XmlUtilities.GetFloatAttribute(xmlNodeList[i], optionValuesAttributeName));

                    if (intOptions[i] == XmlUtilities.GetFloatAttribute(currentOptionNode, optionValuesAttributeName))
                    {
                        startOptionIndex = i;
                    }
                }
                //end option values init
            }

            if (widestOptionToDisplay is List<TextObject> wOptionToDisplay && editableOptionsToDisplay[editableOptionsToDisplay.Count - 1] is TextObject lastEditableOption)
            {
                wOptionToDisplay.Add((TextObject)lastEditableOption.Clone());
            }

            if (widestOptionToDisplay is List<GameObject> _wOptionToDisplay && editableOptionsToDisplay[editableOptionsToDisplay.Count - 1] is GameObject _lastEditableOption)
            {
                _wOptionToDisplay.Add((GameObject)_lastEditableOption.Clone());
            }

            previousOption = new TextObject("<", new Vector2(optionTextObjectInfo.Position.X + optionTextObjectInfo.GetTextWidth() + 0.2f, menuPos.Y - 0.1f), dType, uType, isClickable: true);
            nextOption = new TextObject(">", Vector2.Zero, dType, uType, isClickable: true);
            editableOption = new ComboBoxOption<T, R>(optionTextObjectInfo, nextOption, previousOption, editableOptionsToDisplay, optionValues, startOptionIndex, previousOption.Position + new Vector2(previousOption.GetTextWidth() + 0.2f, 0.1f), uType);

            InitEditableOptionPosition(editableOption, widestOptionToDisplay, ref menuPos);

            if (initGUICamera)
            {
                editableOption.InitOptionsToDisplayGUICamera();
            }

            if (editableOptionsToDisplay is List<TextObject> __optionsToDisplay)
            {
                foreach (TextObject optionToDisplay in __optionsToDisplay)
                {
                    optionToDisplay.RemoveText();
                }
            }

            else if (editableOptionsToDisplay is List<GameObject> _optionsToDisplay)
            {
                foreach (GameObject optionToDisplay in _optionsToDisplay)
                {
                    DrawMngr.Remove(optionToDisplay, dType);
                    UpdateMngr.Remove(optionToDisplay, uType);
                }
            }

            editableOption.IsActive = true;
        }

        private static void InitEditableOptionPosition(ComboBoxOption<T, R> option, List<T> widestOptionToDisplay, ref Vector2 menuPos, bool incmenuPosY = true)
        {
            if (widestOptionToDisplay is List<TextObject> wOptionToDisplay)
            {
                wOptionToDisplay[0].EditPosition(option.OptionsToDisplayPosition);
                option.NextOption.EditPosition(new Vector2(wOptionToDisplay[0].Position.X + wOptionToDisplay[0].GetTextWidth() + 0.2f, option.PreviousOption.Position.Y));
                option.AlignCurrentOptionPosition();

                wOptionToDisplay[0].RemoveText();
            }

            else if (widestOptionToDisplay is List<GameObject> _wOptionToDisplay)
            {
                _wOptionToDisplay[0].Position = option.OptionsToDisplayPosition;
                option.NextOption.EditPosition(new Vector2(_wOptionToDisplay[0].Position.X + _wOptionToDisplay[0].Width + 0.2f, option.PreviousOption.Position.Y));
                option.AlignCurrentOptionPosition();

                DrawMngr.Remove(_wOptionToDisplay[0]);
                UpdateMngr.Remove(_wOptionToDisplay[0]);
            }

            if (incmenuPosY)
            {
                menuPos.Y += 1;
            }
        }

        public void Update()
        {
            Input();
        }
    }
}
