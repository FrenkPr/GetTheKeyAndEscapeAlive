using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownGame
{
    class TileNextScene : TileObject
    {
        private string nextSceneName;

        public TileNextScene(Tileset tileset, int GID, float width, float height, Vector2 position, RigidBodyType rigidBodyType, string nextSceneName) : base(tileset, GID, width, height, position, rigidBodyType)
        {
            this.nextSceneName = nextSceneName;
        }

        public void InitTmxObjectNextScene()
        {
            try
            {
                Scene nextScene = Game.CurrentScene.TmxNextScenes[nextSceneName];
                Game.CurrentScene.TiledMap.TmxObjectNextScene.Add(Position, nextScene);
            }
            catch
            {

            }
        }

        public override void OnCollision(CollisionInfo collisionInfo)
        {
            if (collisionInfo.Collider is Player)
            {
                Game.CurrentScene.NextScene = Game.CurrentScene.TiledMap.TmxObjectNextScene[Position];
                Game.CurrentScene.IsPlaying = false;
            }
        }
    }
}
