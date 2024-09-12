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
    class TmxMap : IDrawable
    {
        private Texture mapTexture;
        private Sprite mapSprite;
        private static Tileset[] tilesets;
        private MapLayer[] mapLayers;
        private TileObjectLayer[] objLayers;
        public readonly Dictionary<Vector2, Scene> TmxObjectNextScene;

        public readonly int NumMapCols;
        public readonly int NumMapRows;

        public float Width { get => mapSprite.Width; }
        public float Height { get => mapSprite.Height; }
        public readonly float TileWidth;
        public readonly float TileHeight;

        public Vector2 Position { get => mapSprite.position; }
        public float X { get => mapSprite.position.X; set => mapSprite.position.X = value; }
        public float Y { get => mapSprite.position.Y; set => mapSprite.position.Y = value; }

        public PathfindingMap PathFindingMap { get; private set; }

        public DrawLayer DrawLayer { get; private set; }

        public TmxMap(int width, int height, int rows, int cols, int tileWidth, int tileHeight)
        {
            NumMapRows = rows;
            NumMapCols = cols;
            TileWidth = Game.PixelsToUnits(tileWidth);
            TileHeight = Game.PixelsToUnits(tileHeight);

            mapTexture = new Texture(width, height);

            //tilesets init
            XmlNodeList tilesetsNodes = Game.CurrentScene.XmlMapDoc.GetElementsByTagName("tileset");
            tilesets = new Tileset[tilesetsNodes.Count];

            for (int i = 0; i < tilesets.Length; i++)
            {
                XmlNode tilesetNode = tilesetsNodes[i];

                string textureId = XmlUtilities.GetStringAttribute(tilesetNode, "name");
                int tilesetFirstGID = XmlUtilities.GetIntAttribute(tilesetNode, "firstgid");
                int tilesetTileWidth = XmlUtilities.GetIntAttribute(tilesetNode, "tilewidth");
                int tilesetTileHeight = XmlUtilities.GetIntAttribute(tilesetNode, "tileheight");
                int tileCount = XmlUtilities.GetIntAttribute(tilesetNode, "tilecount");
                int tilesetCols = XmlUtilities.GetIntAttribute(tilesetNode, "columns");
                int tilesetRows = tileCount / tilesetCols;

                tilesets[i] = new Tileset(textureId, tilesetFirstGID, tilesetCols, tilesetRows, tilesetTileWidth, tilesetTileHeight, 0, 0);
            }
            //end tilesets init


            //map layers init
            XmlNodeList layerNodes = Game.CurrentScene.XmlMapDoc.GetElementsByTagName("layer");
            mapLayers = new MapLayer[layerNodes.Count];

            for (int i = 0; i < mapLayers.Length; i++)
            {
                mapLayers[i] = new MapLayer(layerNodes[i]);
            }
            //end map layers init


            //map init
            mapSprite = new Sprite(Game.PixelsToUnits(mapTexture.Width), Game.PixelsToUnits(mapTexture.Height));
            mapSprite.position = Vector2.Zero;

            byte[] mapBitmap = new byte[mapTexture.Width * mapTexture.Height * 4];
            int bytesPerPixel = 4;
            int mapBitmapRowLength = mapTexture.Width * bytesPerPixel;

            for (int i = 0; i < mapLayers.Length; i++)
            {
                for (int y = 0; y < NumMapRows; y++)
                {
                    for (int x = 0; x < NumMapCols; x++)
                    {
                        int tileId = int.Parse(mapLayers[i].IDs[y * NumMapCols + x]);

                        if (tileId == 0)  //if that tilemap cell isn't drawn
                        {
                            continue;
                        }

                        Tileset tileset = GetTilesetIncludedInID(tileId);
                        Texture tilesetTexture = TextureMngr.GetTexture(tileset.TextureId);

                        byte[] tilesetBitmap = tilesetTexture.Bitmap;
                        int tilesetBitmapRowLength = tileset.TileWidth * tileset.TileCols * bytesPerPixel;
                        int tilesetXOff = tileset.GetAtIndex(tileId).X * bytesPerPixel;
                        int tilesetYOff = tileset.GetAtIndex(tileId).Y * tilesetBitmapRowLength;
                        int tilesetBitmapIndexOffset = tilesetYOff + tilesetXOff;

                        // Get correct mapBitmap's section starting index
                        int mapXOff = x * tileWidth * bytesPerPixel;
                        int mapYOff = y * tileHeight * mapBitmapRowLength;
                        int mapBitmapIndexOffset = mapXOff + mapYOff;

                        for (int j = 0; j < tileHeight; j++)
                        {
                            // How tilesetBitmapIndexInitial increments
                            int tilesetBitmapIndexUpdate = j * tilesetBitmapRowLength;

                            // How mapBitmapIndexInitial increments
                            int mapBitmapIndexUpdate = j * mapBitmapRowLength;

                            // Copy tilesetBitmap's tile section to mapBitmap in correct position
                            Array.Copy(tilesetBitmap,
                                       tilesetBitmapIndexOffset + tilesetBitmapIndexUpdate,
                                       mapBitmap,
                                       mapBitmapIndexOffset + mapBitmapIndexUpdate,
                                       tileWidth * bytesPerPixel);
                        }
                    }
                }

                mapTexture.Update(mapBitmap);
            }
            //end map init


            //object groups init
            XmlNodeList objectGroupNodes = Game.CurrentScene.XmlMapDoc.GetElementsByTagName("objectgroup");
            objLayers = new TileObjectLayer[objectGroupNodes.Count];
            TmxObjectNextScene = new Dictionary<Vector2, Scene>();

            if (CameraMngr.MainCamera == null)
            {
                CameraMngr.Init(null, new CameraLimits(Position.X + Width, Position.X, Position.Y + Height, Position.Y));
                CameraMngr.AddCamera("GUI", new Camera());
            }

            else
            {
                CameraMngr.CameraLimits = new CameraLimits(Position.X + Width, Position.X, Position.Y + Height, Position.Y);
            }

            for (int i = 0; i < objLayers.Length; i++)
            {
                objLayers[i] = new TileObjectLayer(objectGroupNodes[i].ChildNodes);
            }

            //pathfinding map init
            PathFindingMap = new PathfindingMap(NumMapRows, NumMapCols, TileWidth, TileHeight, objLayers);

            DrawLayer = DrawLayer.Background;
            DrawMngr.Add(this);
        }

        public void InitTmxObjectNextScenes()
        {
            for (int i = 0; i < objLayers.Length; i++)
            {
                objLayers[i].InitObjectNextScenes();
            }
        }

        public static Tileset GetTilesetIncludedInID(int tileId)
        {
            for (int i = 0; i < tilesets.Length; i++)
            {
                if (i == 0)
                {
                    if (tileId >= tilesets[i].FirstGID && tileId <= tilesets[i].TileCount)
                    {
                        return tilesets[i];
                    }

                    continue;
                }

                if (tileId >= tilesets[i].FirstGID && tileId <= tilesets[i].FirstGID + tilesets[i].TileCount - tilesets[0].FirstGID)
                {
                    return tilesets[i];
                }
            }

            return null;
        }

        public void Clear()
        {
            mapTexture = null;
            mapSprite = null;
            tilesets = null;
            mapLayers = null;

            foreach (TileObjectLayer objLayer in objLayers)
            {
                objLayer.Clear();
            }

            objLayers = null;
            PathFindingMap.Clear();
            PathFindingMap = null;

            DrawMngr.Remove(this);
        }

        public void Draw()
        {
            mapSprite.DrawTexture(mapTexture);
        }
    }
}
