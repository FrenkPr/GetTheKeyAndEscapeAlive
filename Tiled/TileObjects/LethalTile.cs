using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownGame
{
    class LethalTile : TileObject
    {
        public LethalTile(Tileset tileset, int GID, float width, float height, Vector2 position, RigidBodyType rigidBodyType) : base(tileset, GID, width, height, position, rigidBodyType)
        {
            if(!(this is ToggleableLethalTile))
            {
                RigidBody.Collider.Offset = new Vector2(HalfWidth, HalfHeight);
            }
        }

        public override void OnCollision(CollisionInfo collisionInfo)
        {
            if (collisionInfo.Collider is Player player)
            {
                player.OnDie();
            }
        }
    }
}
