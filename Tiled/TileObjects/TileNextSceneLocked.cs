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
    class TileNextSceneLocked : TileNextScene, IOnChangeWindowSize
    {
        public bool IsLocked { get; private set; }
        private TileOffset tilesetUnlockedOffset;
        private Texture unlockedTileTexture;

        private string keyName;
        public readonly bool PlayerGotTheKey;
        private TextObject keyInfo;

        private XmlNode nextSceneLockedNode;
        public static readonly Dictionary<XmlNode, Tuple<XmlDocument, string, string>> NextSceneLockedNodes = new Dictionary<XmlNode, Tuple<XmlDocument, string, string>>();  //this variable is used to restore default attribute values in Game class
        public static TileNextSceneLocked TileNextSceneLockedCollidedWithPlayer { get; private set; }

        public TileNextSceneLocked(Tileset tilesetLockedObject, Tileset tilesetUnlockedObject, int lockedGID, int unlockedGID, float width, float height, Vector2 position, RigidBodyType rigidBodyType, string nextSceneName, string keyName, bool isLocked) : base(tilesetLockedObject, lockedGID, width, height, position, rigidBodyType, nextSceneName)
        {
            if (tilesetUnlockedObject != null)
            {
                tilesetUnlockedOffset = tilesetUnlockedObject.GetAtIndex(unlockedGID);
                unlockedTileTexture = TextureMngr.GetTexture(tilesetUnlockedObject.TextureId);
            }

            keyInfo = new TextObject("It's closed", new Vector2(Game.ScreenCenter.X, Game.OrthoHeight - 0.5f), charWidth: 45, charHeight: 45);
            keyInfo.EditPosition(new Vector2(keyInfo.Position.X - keyInfo.GetTextWidth() * 0.5f, keyInfo.Position.Y));
            keyInfo.InitGUICamera();
            keyInfo.IsActive = false;

            XmlNode playerGotTheKeyNode = Game.CurrentScene.XmlMapDoc.GetElementsByTagName(keyName)[0].SelectSingleNode("properties").SelectSingleNode("nextSceneLockedObj_playerGotTheKey");
            nextSceneLockedNode = Game.CurrentScene.XmlMapDoc.GetElementsByTagName(keyName)[0].SelectSingleNode("properties").SelectSingleNode("nextSceneLockedObj_isLocked");

            PlayerGotTheKey = XmlUtilities.GetBoolAttribute(playerGotTheKeyNode, "value");

            if (!NextSceneLockedNodeAlreadyAdded())
            {
                NextSceneLockedNodes.Add(nextSceneLockedNode, new Tuple<XmlDocument, string, string>(Game.CurrentScene.XmlMapDoc, Game.CurrentScene.XmlMapDocPath, keyName));
            }

            IsLocked = isLocked;
            this.keyName = keyName;

            OnChangeWindowSizeMngr.Add(this);
            UpdateMngr.Add(this);
            //DebugMngr.AddItem(RigidBody.Collider);
        }

        private bool NextSceneLockedNodeAlreadyAdded()
        {
            foreach (Tuple<XmlDocument, string, string> nextSceneLockedNode in NextSceneLockedNodes.Values)
            {
                if (nextSceneLockedNode.Item3 == keyName)
                {
                    return true;
                }
            }

            return false;
        }

        public void OnChangeWindowSize()
        {
            keyInfo.EditPosition(new Vector2(Game.ScreenCenter.X - keyInfo.GetTextWidth() * 0.5f, Game.OrthoHeight - 0.5f));
        }

        public override void OnCollision(CollisionInfo collisionInfo)
        {
            if (IsLocked && !PlayerGotTheKey)
            {
                keyInfo.EditText("It's closed.");
                keyInfo.EditPosition(new Vector2(Game.ScreenCenter.X - keyInfo.GetTextWidth() * 0.5f, Game.OrthoHeight - 0.5f));
                keyInfo.InitGUICamera();
                keyInfo.IsActive = true;
            }

            else if (IsLocked && PlayerGotTheKey)
            {
                keyInfo.EditText($"Press {Game.KeyboardCtrl.KeyToString(KeyCodeType.Interact)} to open the door.");
                keyInfo.EditPosition(new Vector2(Game.ScreenCenter.X - keyInfo.GetTextWidth() * 0.5f, Game.OrthoHeight - 0.5f));
                keyInfo.InitGUICamera();
                keyInfo.IsActive = true;
                TileNextSceneLockedCollidedWithPlayer = this;
            }

            else if (!IsLocked && collisionInfo.Delta.LengthSquared > 0.095f)
            {
                base.OnCollision(collisionInfo);
            }
        }

        public void UnlockNextSceneLocked()
        {
            IsLocked = false;

            nextSceneLockedNode.Attributes[2].Value = "false";
            Game.CurrentScene.XmlMapDoc.Save(Game.CurrentScene.XmlMapDocPath);
        }

        public override void Update()
        {
            keyInfo.IsActive = false;
            TileNextSceneLockedCollidedWithPlayer = null;
        }

        public override void Draw()
        {
            if (!IsActive || texture == null)
            {
                return;
            }

            if (IsLocked)
            {
                Sprite.DrawTexture(texture, tilesetOffset.X, tilesetOffset.Y, (int)PixelsWidth, (int)PixelsHeight);
            }

            else
            {
                Sprite.DrawTexture(unlockedTileTexture, tilesetUnlockedOffset.X, tilesetUnlockedOffset.Y, (int)PixelsWidth, (int)PixelsHeight);
            }
        }
    }
}
