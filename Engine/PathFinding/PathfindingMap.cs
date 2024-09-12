using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiv.Fast2D;
using OpenTK;

namespace TopDownGame
{
    class PathfindingMap
    {
        Dictionary<Node, Node> cameFrom;        // parents
        Dictionary<Node, int> costSoFar;        // distances
        PriorityQueue frontier;                 // toVisit
        public Node[,] Nodes { get; private set; }

        public PathfindingMap(int rows, int cols, float tileWidth, float tileHeight, TileObjectLayer[] obstacles)
        {
            Nodes = new Node[rows, cols];
            float nodePosX = CameraMngr.CameraLimits.MinX + tileWidth * 0.5f;
            float nodePosY = CameraMngr.CameraLimits.MinY + tileHeight * 0.5f;

            // build Nodes via num tile rows and cols
            for (int cellY = 0; cellY < Nodes.GetLength(0); cellY++, nodePosY += tileHeight)
            {
                for (int cellX = 0; cellX < Nodes.GetLength(1); cellX++, nodePosX += tileWidth)
                {
                    Nodes[cellY, cellX] = new Node(cellX, cellY, nodePosX, nodePosY, 1);
                }

                nodePosX = tileWidth * 0.5f;
            }

            for (int cellY = 0; cellY < Nodes.GetLength(0); cellY++)
            {
                for (int cellX = 0; cellX < Nodes.GetLength(1); cellX++)
                {
                    AddNeighbours(Nodes[cellY, cellX]);
                }
            }

            for (int i = 0; i < obstacles.Length; i++)
            {
                for (int cellY = 0; cellY < Nodes.GetLength(0); cellY++)
                {
                    for (int cellX = 0; cellX < Nodes.GetLength(1); cellX++)
                    {
                        if (obstacles[i].IsTileObstacle(Nodes[cellY, cellX].Position) && Nodes[cellY, cellX].Cost != int.MaxValue)
                        {
                            RemoveNode(Nodes[cellY, cellX].X, Nodes[cellY, cellX].Y);
                        }

                        else if (obstacles[i].IsLethalTile(Nodes[cellY, cellX].Position) /*&& Nodes[cellY, cellX].Cost != int.MaxValue*/ && Nodes[cellY, cellX].Cost != 2)
                        {
                            //RemoveNode(Nodes[cellY, cellX].X, Nodes[cellY, cellX].Y);
                            Nodes[cellY, cellX].Cost = 2;
                        }
                    }
                }
            }
        }

        // Add a Neighbour Node (to the passed one) for each direction if they are valid
        void AddNeighbours(Node node)
        {
            // Checks each direction neighbour for the current Node

            // TOP
            CheckNeighbours(node, node.CellX, node.CellY - 1);
            // BOTTOM
            CheckNeighbours(node, node.CellX, node.CellY + 1);
            // RIGHT
            CheckNeighbours(node, node.CellX + 1, node.CellY);
            // LEFT
            CheckNeighbours(node, node.CellX - 1, node.CellY);
        }

        // Checks if the Node at the passed X and Y is inside the map and it's valid and
        // if it is, adds it to the current Node's neighbours
        public void CheckNeighbours(Node currentNode, int cellX, int cellY)
        {
            Node neighbour = GetNodeAtCell(cellX, cellY);

            if (neighbour != null && neighbour.Cost != int.MaxValue)
            {
                currentNode.AddNeighbour(neighbour);
            }
        }

        public void AddNode(float x, float y, int cost = 1)
        {
            Node node = GetNodeAtPos(x, y);

            AddNeighbours(node);

            foreach (Node adj in node.Neighbours)
            {
                adj.AddNeighbour(node);
            }

            node.Cost = cost;
        }

        public void RemoveNode(float x, float y)
        {
            Node node = GetNodeAtPos(x, y);

            foreach (Node adj in node.Neighbours)
            {
                adj.RemoveNeighbour(node);
            }

            node.Cost = int.MaxValue;
        }

        public Node GetNodeAtPos(float x, float y)
        {
            if (x >= CameraMngr.CameraLimits.MaxX || x < CameraMngr.CameraLimits.MinX || y >= CameraMngr.CameraLimits.MaxY || y < CameraMngr.CameraLimits.MinY)
            {
                return null;
            }

            for (int cellY = 0; cellY < Nodes.GetLength(0); cellY++)
            {
                for (int cellX = 0; cellX < Nodes.GetLength(1); cellX++)
                {
                    Vector2 dist = new Vector2(x, y) - Nodes[cellY, cellX].Position;

                    if (dist.LengthSquared <= 0.2f * 0.2f)
                    {
                        //Console.WriteLine("Node cellX: " + cellX);
                        //Console.WriteLine("Node cellY: " + cellY);

                        return Nodes[cellY, cellX];
                    }
                }
            }

            return null;
        }

        public Node GetNodeAtCell(int cellX, int cellY)
        {
            if (cellX >= Nodes.GetLength(1) || cellX < 0 || cellY >= Nodes.GetLength(0) || cellY < 0)
            {
                return null;
            }

            return Nodes[cellY, cellX];
        }

        public List<Node> GetPath(float startX, float startY, float endX, float endY)
        {
            List<Node> path = new List<Node>();

            Node start = GetNodeAtPos(startX, startY);
            Node end = GetNodeAtPos(endX, endY);

            if (start == null || end == null)
            {
                return path;
            }

            if (start.Cost == int.MaxValue || end.Cost == int.MaxValue)
            {
                return path;
            }

            AStar(start, end);

            if (!cameFrom.ContainsKey(end))
            {
                return path;
            }

            Node currNode = end;

            while (currNode != cameFrom[currNode])
            {
                path.Add(currNode);
                currNode = cameFrom[currNode];
            }

            path.Reverse();

            return path;
        }

        public void AStar(Node start, Node end)
        {
            cameFrom = new Dictionary<Node, Node>();
            costSoFar = new Dictionary<Node, int>();
            frontier = new PriorityQueue();

            cameFrom[start] = start;
            costSoFar[start] = 0;
            frontier.Enqueue(start, Heuristic(start, end));

            while (!frontier.IsEmpty)
            {
                Node currNode = frontier.Dequeue();

                if (currNode == end)
                {
                    return;
                }

                foreach (Node nextNode in currNode.Neighbours)
                {
                    int newCost = costSoFar[currNode] + nextNode.Cost;

                    if ((!costSoFar.ContainsKey(nextNode) || costSoFar[nextNode] > newCost) && costSoFar[currNode] != int.MaxValue)
                    {
                        cameFrom[nextNode] = currNode;
                        costSoFar[nextNode] = newCost;
                        float priority = newCost + Heuristic(nextNode, end);
                        frontier.Enqueue(nextNode, priority);
                    }
                }
            }
        }

        private float Heuristic(Node start, Node end)
        {
            return Math.Abs(start.X - end.X) + Math.Abs(start.Y - end.Y);  // Manhattan Distance
        }

        public void Clear()
        {
            cameFrom = null;
            costSoFar = null;
            frontier = null;
            Nodes = null;
        }
    }
}
