using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiv.Fast2D;
using OpenTK;

namespace TopDownGame
{
    class Agent : IDrawable
    {
        public Node Target;
        public Node CurrentNode;
        public List<Node> Path { get; private set; }

        private Actor actor;

        // Sprite and Color to Draw Path Nodes
        private Sprite pathSpr;
        private Vector4 pathCol;

        public DrawLayer DrawLayer { get; private set; }

        public Agent(Actor owner, Vector4 pathColor)
        {
            actor = owner;
            Target = null;

            pathSpr = new Sprite(actor.HalfWidth, actor.HalfHeight);
            pathSpr.pivot = new Vector2(pathSpr.Width * 0.5f, pathSpr.Height * 0.5f);
            pathCol = pathColor;

            DrawLayer = DrawLayer.GUI;
            //DrawMngr.Add(this);
        }

        public virtual void SetPath(List<Node> newPath)
        {
            Path = newPath;

            // Can't reach target but have a path, goes to first path node
            if (Target == null && Path.Count > 0)
            {
                Target = Path[0];
                Path.RemoveAt(0);
            }

            // Can reach target
            else if (Path.Count > 0)
            {
                // Heuristic between current Node and target Node
                float dist = Math.Abs(Path[0].X - Target.X) + Math.Abs(Path[0].Y - Target.Y);

                // if dist > 1 means we're actually jumping a Node (diag),
                // so we add it again
                if (dist > 1)
                {
                    Path.Insert(0, CurrentNode);
                }
            }
        }

        public void ResetPath()
        {
            if (Path != null)
            {
                Path.Clear();
            }

            Target = null;
        }

        public Node GetLastNode()
        {
            if (Path.Count > 0)
            {
                return Path.Last();
            }

            return null;
        }

        public void SetActorForward(Vector2 TargetActorDist)
        {
            //Set horizontal forward
            if (Math.Abs(TargetActorDist.X) > Math.Abs(TargetActorDist.Y))
            {
                actor.Forward.X = (float)(Math.Ceiling(Math.Abs(TargetActorDist.X)) * Math.Sign(TargetActorDist.X));
                actor.Forward.Y = 0;
            }

            //Set vertical forward
            else
            {
                actor.Forward.X = 0;
                actor.Forward.Y = (float)(Math.Ceiling(Math.Abs(TargetActorDist.Y)) * Math.Sign(TargetActorDist.Y));
            }
        }

        public virtual void Update()
        {
            if (Target == null || Path == null)
            {
                return;
            }

            Vector2 direction = (Target.Position - actor.Position);
            float distance = direction.LengthSquared;
            
            direction.Normalize();

            if (distance < 0.05f * 0.05f)
            {
                CurrentNode = Target;
                actor.X = Vector2.Lerp(actor.Position, Target.Position, actor.RigidBody.Velocity.X * Game.DeltaTime).X;
                actor.Y = Vector2.Lerp(actor.Position, Target.Position, actor.RigidBody.Velocity.Y * Game.DeltaTime).Y;

                if (Path.Count == 0)
                {
                    Target = null;
                    actor.LastForward = actor.Forward;
                }

                else
                {
                    Target = Path[0];
                    Path.RemoveAt(0);

                    SetActorForward(direction);
                }
            }

            else
            {
                actor.Position += actor.RigidBody.Velocity * direction * Game.DeltaTime;

                SetActorForward(direction);
            }
        }

        public void Draw()
        {
            // Draw Path
            if (Path == null)
            {
                return;
            }

            foreach (Node n in Path)
            {
                pathSpr.position = new Vector2(n.X, n.Y);
                pathSpr.DrawColor(pathCol);
            }
        }
    }
}
