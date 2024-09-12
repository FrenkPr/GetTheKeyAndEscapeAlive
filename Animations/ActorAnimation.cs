using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownGame
{
    class ActorAnimation : GameObject
    {
        private Animation animation;
        private Actor actor;

        public ActorAnimation(Actor owner, string textureId, int numFrames, bool flippedX = false, bool loop = true) : base(textureId, numFrames)
        {
            animation = new Animation(this, 12, loop);
            actor = owner;
            IsActive = true;
            animation.IsEnabled = true;

            Sprite.FlipX = flippedX;

            UpdateMngr.Remove(this);
            DrawMngr.Remove(this);
        }

        public override void Update()
        {
            if (animation.IsEnabled)
            {
                Position = actor.Position;  //put the animation sprite position to the player position

                if (NumFrames > 1)
                {
                    animation.Update();
                }
            }
        }

        public override void Draw()
        {
            if (animation.IsEnabled)
            {
                base.Draw();
            }
        }
    }
}
