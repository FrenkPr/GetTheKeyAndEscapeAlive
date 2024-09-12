using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownGame
{
    class PriorityQueue
    {
        private Dictionary<Node, float> items;
        public bool IsEmpty { get { return items.Count == 0; } }

        public PriorityQueue()
        {
            items = new Dictionary<Node, float>();
        }

        public void Enqueue(Node node, float priority)
        {
            if (!items.ContainsKey(node) && node.Cost != int.MaxValue)
            {
                items.Add(node, priority);
            }
        }

        public Node Dequeue()
        {
            float lowestPriority = int.MaxValue;
            Node node = null;

            foreach (Node item in items.Keys)
            {
                float currentPriority = items[item];

                if (currentPriority < lowestPriority)
                {
                    lowestPriority = currentPriority;
                    node = item;
                }
            }

            items.Remove(node);

            return node;
        }
    }
}
