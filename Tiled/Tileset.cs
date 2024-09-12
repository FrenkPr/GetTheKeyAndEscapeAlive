using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownGame
{
    struct TileOffset
    {
        public int X;
        public int Y;

        public TileOffset(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    class Tileset
    {
        private TileOffset[] tiles;
        public string TextureId { get; }
        public int FirstGID { get; }
        public int TileCount { get; }
        public int TileCols { get; }
        public int TileRows { get; }
        public int TileWidth { get; }
        public int TileHeight { get; }

        public Tileset(string textureId, int firstGID, int cols, int rows, int tileWidth, int tileHeight, int margin, int spacing)
        {
            tiles = new TileOffset[rows * cols];

            TextureId = textureId;
            FirstGID = firstGID;
            TileCount = tiles.Length;
            TileCols = cols;
            TileRows = rows;
            TileWidth = tileWidth;
            TileHeight = tileHeight;

            int xOffset = margin;
            int yOffset = margin;

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    tiles[y * cols + x] = new TileOffset(xOffset, yOffset);

                    xOffset += TileWidth + spacing;
                }

                xOffset = margin;
                yOffset += TileHeight + spacing;
            }
        }

        public TileOffset GetAtIndex(int index)
        {
            return tiles[index - FirstGID];
        }
    }
}
