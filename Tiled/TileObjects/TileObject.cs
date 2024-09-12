using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownGame
{
    class TileObject : GameObject
    {
        protected TileOffset tilesetOffset;

        public TileObject(Tileset tileset, int GID, float width, float height, Vector2 position, RigidBodyType rigidBodyType) : base(tileset == null ? "" : tileset.TextureId, 1, width, height)
        {
            if (tileset != null)
            {
                tilesetOffset = tileset.GetAtIndex(GID);
            }

            Position = new Vector2(Game.PixelsToUnits(position.X), Game.PixelsToUnits(position.Y));

            RigidBody = new RigidBody(this, Vector2.Zero);
            RigidBody.Type = rigidBodyType;
            RigidBody.Collider = ColliderFactory.CreateBoxFor(this);
            RigidBody.AddCollisionType(RigidBodyType.Player);

            if (texture != null)
            {
                Sprite.pivot = new Vector2(0, Height);

                if (RigidBody.Collider.Offset == Vector2.Zero)
                {
                    RigidBody.Collider.Offset = new Vector2(HalfWidth * 0.5f, -HalfHeight * 0.5f);
                }
            }

            if (RigidBody.Collider.Offset == Vector2.Zero)
            {
                RigidBody.Collider.Offset = new Vector2(HalfWidth * 0.5f, HalfHeight * 0.5f);
            }

            UpdateMngr.Remove(this);
            //DebugMngr.AddItem(RigidBody.Collider);

            IsActive = true;
        }

        public override void Draw()
        {
            if (IsActive && texture != null)
            {
                Sprite.DrawTexture(texture, tilesetOffset.X, tilesetOffset.Y, (int)PixelsWidth, (int)PixelsHeight);
            }
        }
    }
}
