using Aiv.Fast2D;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownGame
{
    class ToggleableLethalTile : LethalTile
    {
        private bool isLethalActive;
        private TileOffset tilesetActiveOffset;
        private Texture activeTileTexture;
        private Timer timeToToggleLethal;

        public ToggleableLethalTile(Tileset tilesetDisactive, Tileset tilesetActive, int disactiveGID, int activeGID, float width, float height, Vector2 position, RigidBodyType rigidBodyType, float maxTimeToToggleLethal) : base(tilesetDisactive, disactiveGID, width, height, position, rigidBodyType)
        {
            if (tilesetActive != null)
            {
                tilesetActiveOffset = tilesetActive.GetAtIndex(activeGID);
                activeTileTexture = TextureMngr.GetTexture(tilesetActive.TextureId);
            }

            isLethalActive = false;
            timeToToggleLethal = new Timer(maxTimeToToggleLethal);

            UpdateMngr.Add(this);
        }

        public override void OnCollision(CollisionInfo collisionInfo)
        {
            if (isLethalActive && collisionInfo.Delta.LengthSquared > 0.07f)  //we chek delta length squared to don't let the player die when next to lethal tile
            {
                base.OnCollision(collisionInfo);
            }
        }

        public override void Update()
        {
            if (IsActive)
            {
                timeToToggleLethal.DecTime();

                if (timeToToggleLethal.Clock <= 0)
                {
                    isLethalActive = !isLethalActive;
                    timeToToggleLethal.Reset();
                }
            }
        }

        public override void Draw()
        {
            if (!IsActive || texture == null)
            {
                return;
            }

            if (isLethalActive)
            {
                Sprite.DrawTexture(activeTileTexture, tilesetActiveOffset.X, tilesetActiveOffset.Y, (int)PixelsWidth, (int)PixelsHeight);
            }

            else
            {
                Sprite.DrawTexture(texture, tilesetOffset.X, tilesetOffset.Y, (int)PixelsWidth, (int)PixelsHeight);
            }
        }
    }
}
