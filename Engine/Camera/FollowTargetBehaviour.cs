using Aiv.Fast2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownGame
{
    class FollowTargetBehaviour : CameraBehaviour
    {
        public GameObject Target;
        public float CameraSpeed = 5;

        public FollowTargetBehaviour(Camera camera, GameObject target) : base(camera)
        {
            Target = target;
        }

        public override void Update()
        {
            if (Target != null)
            {
                PointToFollow = Target.Position;
                blendFactor = Game.DeltaTime * CameraSpeed;

                base.Update();
            }
        }
    }
}
