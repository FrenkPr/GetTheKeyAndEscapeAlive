using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiv.Fast2D;
using OpenTK;

namespace TopDownGame
{
    class Node : IDrawable
    {
        public int CellX { get; private set; }
        public int CellY { get; private set; }
        public float X { get; private set; }
        public float Y { get; private set; }
        public Vector2 Position { get => new Vector2(X, Y); }

        public int Cost;

        public List<Node> Neighbours { get; }
        private Sprite spr;
        public DrawLayer DrawLayer { get; private set; }

        public Node(int cellX, int cellY, float x, float y, int cost)
        {
            CellX = cellX;
            CellY = cellY;
            X = x;
            Y = y;

            Cost = cost;

            Neighbours = new List<Node>();

            DrawLayer = DrawLayer.GUI;

            //DrawMngr.Add(this);
        }

        public void AddNeighbour(Node node)
        {
            Neighbours.Add(node);
        }

        public void RemoveNeighbour(Node node)
        {
            Neighbours.Remove(node);
        }

        public void Draw()
        {
            //if (spr == null)
            //{
            //    spr = new Sprite(Game.CurrentScene.TiledMap.TileWidth, Game.CurrentScene.TiledMap.TileHeight);
            //    spr.pivot = new Vector2(spr.Width * 0.5f, spr.Height * 0.5f);
            //    spr.position = new Vector2(X, Y);
            //}

            //if (spr != null&&spr.position==spr.pivot)
            //{
            //    spr.DrawColor(new Vector4(1, 0, 0, 1));
            //}
        }
    }
}
