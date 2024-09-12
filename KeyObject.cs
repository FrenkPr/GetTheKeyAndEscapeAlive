using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TopDownGame
{
    class KeyObject : GameObject, IOnChangeWindowSize
    {
        private string pathMapRelativeToNextSceneLocked;
        private XmlDocument xmlMapDocRelativeToNextSceneLocked;

        private XmlNode playerGotTheKeyNode;

        private TextObject keyGotInfo;
        private Timer timeToDeactivateKeyGotInfo;

        private string keyName;

        public static readonly Dictionary<XmlNode, Tuple<XmlDocument, string, string>> PlayerGotTheKeyNodes = new Dictionary<XmlNode, Tuple<XmlDocument, string, string>>();  //this variable is used to restore default attribute values in Game class

        public KeyObject(string pathMapRelativeToNextSceneLocked, string keyName) : base("key", 1, 32, 32)
        {
            RigidBody = new RigidBody(this, Vector2.Zero);
            RigidBody.Collider = ColliderFactory.CreateBoxFor(this);
            RigidBody.Type = RigidBodyType.KeyObject;
            RigidBody.AddCollisionType(RigidBodyType.Player);

            keyGotInfo = new TextObject("You got a key!", Game.ScreenCenter + new Vector2(0, 3), charWidth: 45, charHeight: 45);
            keyGotInfo.EditPosition(new Vector2(keyGotInfo.Position.X - keyGotInfo.GetTextWidth() * 0.5f, keyGotInfo.Position.Y));
            keyGotInfo.InitGUICamera();
            keyGotInfo.IsActive = false;

            this.pathMapRelativeToNextSceneLocked = pathMapRelativeToNextSceneLocked;
            xmlMapDocRelativeToNextSceneLocked = Game.InitXmlDoc(pathMapRelativeToNextSceneLocked);

            timeToDeactivateKeyGotInfo = new Timer(5, 5);

            playerGotTheKeyNode = xmlMapDocRelativeToNextSceneLocked.GetElementsByTagName(keyName)[0].SelectSingleNode("properties").SelectSingleNode("nextSceneLockedObj_playerGotTheKey");
            this.keyName = keyName;

            IsActive = !XmlUtilities.GetBoolAttribute(playerGotTheKeyNode, "value");

            if (!PlayerGotTheKeyNodeAlreadyAdded())
            {
                PlayerGotTheKeyNodes.Add(playerGotTheKeyNode, new Tuple<XmlDocument, string, string>(xmlMapDocRelativeToNextSceneLocked, pathMapRelativeToNextSceneLocked, keyName));
            }

            OnChangeWindowSizeMngr.Add(this);
        }

        private bool PlayerGotTheKeyNodeAlreadyAdded()
        {
            foreach (Tuple<XmlDocument, string, string> playerGotTheKeyNode in PlayerGotTheKeyNodes.Values)
            {
                if (playerGotTheKeyNode.Item3 == keyName)
                {
                    return true;
                }
            }

            return false;
        }

        public override void OnCollision(CollisionInfo collisionInfo)
        {
            IsActive = false;
            keyGotInfo.IsActive = true;

            playerGotTheKeyNode.Attributes[2].Value = "true";
            xmlMapDocRelativeToNextSceneLocked.Save(pathMapRelativeToNextSceneLocked);
        }

        public void OnChangeWindowSize()
        {
            keyGotInfo.EditPosition(new Vector2(Game.ScreenCenter.X - keyGotInfo.GetTextWidth() * 0.5f, Game.ScreenCenter.Y + 3));
        }

        public override void Update()
        {
            if (!keyGotInfo.IsActive)
            {
                return;
            }

            timeToDeactivateKeyGotInfo.DecTime();

            if (timeToDeactivateKeyGotInfo.Clock <= 0)
            {
                keyGotInfo.IsActive = false;
            }
        }
    }
}
