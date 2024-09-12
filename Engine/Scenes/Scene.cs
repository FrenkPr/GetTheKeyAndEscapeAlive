using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TopDownGame
{
    abstract class Scene
    {
        public bool IsPlaying;
        public Scene NextScene;
        public Dictionary<string, Scene> TmxNextScenes { get; protected set; }
        public bool GameEnded;

        public string XmlMapDocPath { get; private set; }
        public XmlDocument XmlMapDoc { get; private set; }
        public TmxMap TiledMap { get; protected set; }

        public PauseMenu PauseMenu { get; protected set; }

        public Scene()
        {

        }

        public virtual void Start()
        {
            PauseMenu = new PauseMenu();

            PauseMenu.PlayerInputsLocked = true;
            IsPlaying = true;
        }

        protected virtual void LoadAssets()
        {
            TextureMngr.AddTexture("mapTileset", "Assets/TILESET/PixelPackTOPDOWN8BIT.png");

            XmlNode playerTypeNode = Game.XmlGameConfigDoc.GetElementsByTagName("currentPlayerType")[0];
            string playerType = XmlUtilities.GetStringAttribute(playerTypeNode, "name");

            //ACTOR IDLE TEXTURES
            TextureMngr.AddTexture("playerIdleU", $"Assets/SPRITES/HEROS/spritesheets/{playerType}/HEROS8bit_{playerType} Idle U.png");
            TextureMngr.AddTexture("playerIdleD", $"Assets/SPRITES/HEROS/spritesheets/{playerType}/HEROS8bit_{playerType} Idle D.png");
            TextureMngr.AddTexture("playerIdleR", $"Assets/SPRITES/HEROS/spritesheets/{playerType}/HEROS8bit_{playerType} Idle R.png");

            //ACTOR WALK TEXTURES
            TextureMngr.AddTexture("playerWalkU", $"Assets/SPRITES/HEROS/spritesheets/{playerType}/HEROS8bit_{playerType} Walk U.png");
            TextureMngr.AddTexture("playerWalkD", $"Assets/SPRITES/HEROS/spritesheets/{playerType}/HEROS8bit_{playerType} Walk D.png");
            TextureMngr.AddTexture("playerWalkR", $"Assets/SPRITES/HEROS/spritesheets/{playerType}/HEROS8bit_{playerType} Walk R.png");

            FontMngr.AddFont("comics", "comics", "Assets/FONTS/comics.png", 10, 10, 32);
        }

        protected void InitTiledMap(string path)
        {
            XmlMapDocPath = path;
            XmlMapDoc = Game.InitXmlDoc(path);

            XmlNode mapNode = XmlMapDoc.SelectSingleNode("map");

            int mapCols = XmlUtilities.GetIntAttribute(mapNode, "width");
            int mapRows = XmlUtilities.GetIntAttribute(mapNode, "height");

            int mapTileWidth = XmlUtilities.GetIntAttribute(mapNode, "tilewidth");
            int mapTileHeight = XmlUtilities.GetIntAttribute(mapNode, "tileheight");
            int mapWidth = mapTileWidth * mapCols;
            int mapHeight = mapTileHeight * mapRows;

            mapWidth = mapWidth < Game.WindowWidth ? Game.WindowWidth : mapWidth;
            mapHeight = mapHeight < Game.WindowHeight ? Game.WindowHeight : mapHeight;

            TiledMap = new TmxMap(mapWidth, mapHeight, mapRows, mapCols, mapTileWidth, mapTileHeight);
        }

        public virtual void Update()
        {
            PauseMenu.Input();
            PauseMenu.Update();

            if (PauseMenu.IsActive)
            {
                UpdateMngr.Update(UpdatingType.PauseMenu);
                DrawMngr.Draw(DrawingType.PauseMenu);
            }
        }

        public abstract void OnExit();
    }
}
