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
    class TileObjectLayer
    {
        public TileObject[] TileObjects { get; private set; }

        public TileObjectLayer(XmlNodeList objectNodes)
        {
            TileObjects = new TileObject[objectNodes.Count];

            for (int i = 0; i < TileObjects.Length; i++)
            {
                Dictionary<RigidBodyType, Type> tileObjectTypes = new Dictionary<RigidBodyType, Type>((int)RigidBodyType.TileTypesLength);
                Dictionary<RigidBodyType, object[]> tileObjectArgs = new Dictionary<RigidBodyType, object[]>((int)RigidBodyType.TileTypesLength);

                //tile object rigidBody property type
                XmlNode rigidBodyTypeNode = objectNodes[i].SelectSingleNode("properties").SelectSingleNode("rigidBodyType");

                //tile next scene property nodes
                XmlNode nextSceneObj_nextSceneNameNode = objectNodes[i].SelectSingleNode("properties").SelectSingleNode("nextSceneObj_nextSceneName");

                //tile next scene locked property nodes
                XmlNode nextSceneLockedObj_isLockedNode = objectNodes[i].SelectSingleNode("properties").SelectSingleNode("nextSceneLockedObj_isLocked");
                XmlNode nextSceneLockedObj_GIDUnlockedNode = objectNodes[i].SelectSingleNode("properties").SelectSingleNode("nextSceneLockedObj_GIDUnlocked");
                XmlNode nextSceneLockedObj_keyNameNode = objectNodes[i].SelectSingleNode("properties").SelectSingleNode("nextSceneLockedObj_keyName");

                //lethal and toggleable lethal tile property nodes
                XmlNode toggleableLethalObj_GIDActiveNode = objectNodes[i].SelectSingleNode("properties").SelectSingleNode("toggleableLethalObj_GIDActive");
                XmlNode toggleableLethalObj_toggleActivationTimeNode = objectNodes[i].SelectSingleNode("properties").SelectSingleNode("toggleableLethalObj_toggleActivationTime");

                //TILE OBJECT ATTRIBUTES
                RigidBodyType rigidBodyType = (RigidBodyType)XmlUtilities.GetIntAttribute(rigidBodyTypeNode, "value");

                int tileObjectGID = XmlUtilities.GetIntAttribute(objectNodes[i], "gid");
                Tileset tileObjectTileset = TmxMap.GetTilesetIncludedInID(tileObjectGID);

                Vector2 tileObjectPosition = new Vector2(XmlUtilities.GetFloatAttribute(objectNodes[i], "x"), XmlUtilities.GetFloatAttribute(objectNodes[i], "y"));
                float tileObjectWidth = XmlUtilities.GetFloatAttribute(objectNodes[i], "width");
                float tileObjectHeight = XmlUtilities.GetFloatAttribute(objectNodes[i], "height");

                float tileObjectRotation = XmlUtilities.GetFloatAttribute(objectNodes[i], "rotation");


                //TILE NEXT SCENE ATTRIBUTE
                string nextSceneObj_nextSceneName = XmlUtilities.GetStringAttribute(nextSceneObj_nextSceneNameNode, "value");


                //TILE NEXT SCENE LOCKED ATTRIBUTES
                bool nextSceneLockedObj_isLocked = XmlUtilities.GetBoolAttribute(nextSceneLockedObj_isLockedNode, "value");
                int nextSceneLockedObj_GIDUnlocked = XmlUtilities.GetIntAttribute(nextSceneLockedObj_GIDUnlockedNode, "value");
                string nextSceneLockedObj_keyName = XmlUtilities.GetStringAttribute(nextSceneLockedObj_keyNameNode, "value");


                //TOGGLEABLE LETHAL TILE ATTRIBUTES
                int toggleableLethalObj_GIDActive = XmlUtilities.GetIntAttribute(toggleableLethalObj_GIDActiveNode, "value");
                float toggleableLethalObj_toggleActivationTime = XmlUtilities.GetFloatAttribute(toggleableLethalObj_toggleActivationTimeNode, "value");


                //TILE OBJECT TYPES FOR ACTIVATOR CREATE INSTANCE
                tileObjectTypes[RigidBodyType.SimpleTileObject] = typeof(TileObject);
                tileObjectTypes[RigidBodyType.InfiniteNodeCost] = typeof(TileObject);

                tileObjectTypes[RigidBodyType.TileNextScene] = typeof(TileNextScene);
                tileObjectTypes[RigidBodyType.TileNextSceneLocked] = typeof(TileNextSceneLocked);

                tileObjectTypes[RigidBodyType.TileLethalObject] = typeof(LethalTile);
                tileObjectTypes[RigidBodyType.TileToggleableLethalObject] = typeof(ToggleableLethalTile);


                //TILE OBJECTS PARAMETERS FOR ACTIVATOR CREATE INSTANCE
                tileObjectArgs[RigidBodyType.SimpleTileObject] = new object[] { tileObjectTileset, tileObjectGID, tileObjectWidth, tileObjectHeight, tileObjectPosition, rigidBodyType };
                tileObjectArgs[RigidBodyType.InfiniteNodeCost] = new object[] { tileObjectTileset, tileObjectGID, tileObjectWidth, tileObjectHeight, tileObjectPosition, rigidBodyType };

                tileObjectArgs[RigidBodyType.TileNextScene] = new object[] { tileObjectTileset, tileObjectGID, tileObjectWidth, tileObjectHeight, tileObjectPosition, rigidBodyType, nextSceneObj_nextSceneName };
                tileObjectArgs[RigidBodyType.TileNextSceneLocked] = new object[] { tileObjectTileset, tileObjectTileset, tileObjectGID, nextSceneLockedObj_GIDUnlocked, tileObjectWidth, tileObjectHeight, tileObjectPosition, rigidBodyType, nextSceneObj_nextSceneName, nextSceneLockedObj_keyName, nextSceneLockedObj_isLocked };

                tileObjectArgs[RigidBodyType.TileLethalObject] = new object[] { tileObjectTileset, tileObjectGID, tileObjectWidth, tileObjectHeight, tileObjectPosition, rigidBodyType };
                tileObjectArgs[RigidBodyType.TileToggleableLethalObject] = new object[] { tileObjectTileset, tileObjectTileset, tileObjectGID, toggleableLethalObj_GIDActive, tileObjectWidth, tileObjectHeight, tileObjectPosition, rigidBodyType, toggleableLethalObj_toggleActivationTime };

                //creating tile object
                TileObjects[i] = (TileObject)Activator.CreateInstance(tileObjectTypes[rigidBodyType], tileObjectArgs[rigidBodyType]);
                TileObjects[i].Sprite.EulerRotation = tileObjectRotation;

                if (TileObjects[i].Sprite.Rotation != 0)
                {
                    TileObjects[i].RigidBody.Collider.Offset.X += TileObjects[i].HalfWidth * (float)Math.Cos(TileObjects[i].Sprite.Rotation);
                    TileObjects[i].RigidBody.Collider.Offset.Y += TileObjects[i].HalfHeight * (float)Math.Sin(TileObjects[i].Sprite.Rotation);
                }
            }
        }

        public void InitObjectNextScenes()
        {
            for (int i = 0; i < TileObjects.Length; i++)
            {
                if (TileObjects[i] is TileNextScene tile)
                {
                    tile.InitTmxObjectNextScene();
                }
            }
        }

        public bool IsTileObstacle(Vector2 point)
        {
            foreach (TileObject obj in TileObjects)
            {
                if (obj.RigidBody.Collider.Contains(point))
                {
                    if (obj.RigidBody.Type == RigidBodyType.InfiniteNodeCost)
                    {
                        return true;
                    }

                    else if (obj is TileNextSceneLocked nextSceneLocked && nextSceneLocked.IsLocked)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool IsLethalTile(Vector2 point)
        {
            foreach (TileObject obj in TileObjects)
            {
                if (obj.RigidBody.Type == RigidBodyType.TileLethalObject)
                {
                    if (point.X >= obj.Position.X &&
                        point.X <= obj.Position.X + obj.Width &&
                        point.Y >= obj.Position.Y &&
                        point.Y <= obj.Position.Y + obj.Height)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void Clear()
        {
            TileObjects = null;
        }
    }
}
