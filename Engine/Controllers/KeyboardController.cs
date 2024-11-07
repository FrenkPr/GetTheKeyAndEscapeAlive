using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Aiv.Fast2D;

namespace TopDownGame
{
    enum KeyCodeType
    {
        Interact,
        Confirm,
        GoBack,
        TogglePauseMenu
    }

    struct KeyboardConfig
    {
        public Dictionary<KeyCodeType, KeyCode> KeyCode;

        public KeyboardConfig(List<KeyCode> keys)
        {
            KeyCode = new Dictionary<KeyCodeType, KeyCode>();

            for (int i = 0; i < keys.Count; i++)
            {
                KeyCode.Add((KeyCodeType)i, keys[i]);
            }
        }
    }

    class KeyboardController : Controller, IUpdatable
    {
        public readonly KeyboardConfig Keys;
        public static KeyCode LastKeyPressed { get; private set; }
        public static KeyCode LastKeyReleased { get; private set; }
        public static bool IsLastKeyPressedHold { get; private set; }
        private Timer isKeyHoldTimer;
        public Dictionary<KeyCode, string> KeyString;

        public KeyboardController(int index, List<KeyCode> keys) : base(index)
        {
            Keys = new KeyboardConfig(keys);
            isKeyHoldTimer = new Timer(0.5f, 0.5f);

            KeyString = new Dictionary<KeyCode, string>((int)KeyCode.Tab);

            XmlNode keyBindingsNode = Game.XmlGameConfigDoc.GetElementsByTagName("keyBindings")[0];

            string keyBindingsText = keyBindingsNode.InnerText;
            keyBindingsText = keyBindingsText.Replace("\r\n", "").Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("_", " ");

            string[] keyBindings = keyBindingsText.Split(',', '=');

            for (int i = 0; i < keyBindings.Length; i += 2)
            {
                try
                {
                    KeyString[(KeyCode)int.Parse(keyBindings[i + 1])] = keyBindings[i];
                }

                catch (Exception)
                {
                    //Console.WriteLine(Keys.KeyCode[KeyCodeType.Interact]);
                    //Console.WriteLine(keyBindings[i + 1]);
                    KeyString[(KeyCode)Convert.ToInt32(keyBindings[i + 1], 16)] = keyBindings[i];
                }
            }
        }

        public string KeyToString(KeyCodeType keyBinded)
        {
            return Game.KeyboardCtrl.KeyString[Game.KeyboardCtrl.Keys.KeyCode[keyBinded]];
        }

        //key holding dipendent from keyboard controllers
        public bool OnKeyPressed(KeyCodeType value)
        {
            return OnKeyPressed(Keys.KeyCode[value]);
        }

        //key holding indipendent from keyboard controllers
        private static bool OnKeyPressed(KeyCode value)
        {
            if (Game.Window.GetKey(value))
            {
                LastKeyPressed = value;
                return true;
            }

            else if (!Game.Window.GetKey(LastKeyPressed))
            {
                LastKeyReleased = LastKeyPressed;
            }

            return false;
        }

        public bool OnKeyHold(KeyCodeType value)
        {
            return OnKeyHold(Keys.KeyCode[value]);
        }

        public static bool OnKeyHold(KeyCode value)
        {
            return OnKeyPressed(value) && IsLastKeyPressedHold;
        }

        public bool OnKeyRelease(KeyCodeType value)
        {
            return OnKeyRelease(Keys.KeyCode[value]);
        }

        private static bool OnKeyRelease(KeyCode value)
        {
            return !OnKeyPressed(value) && LastKeyReleased == value;
        }

        public void Update()
        {
            if (OnKeyPressed(LastKeyPressed))
            {
                isKeyHoldTimer.DecTime();

                if (isKeyHoldTimer.Clock <= 0)
                {
                    IsLastKeyPressedHold = true;
                }
            }

            else
            {
                isKeyHoldTimer.Reset();
                IsLastKeyPressedHold = false;
            }
        }
    }
}
