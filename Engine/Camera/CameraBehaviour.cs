using Aiv.Fast2D;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownGame
{
    enum CameraBehaviourType
    {
        FollowTarget,
        FollowPoint,
        MoveToPoint,
        Length
    }

    abstract class CameraBehaviour
    {
        protected Camera camera;
        public Vector2 PointToFollow { get; protected set; }
        protected float blendFactor;

        public CameraBehaviour(Camera camera)
        {
            this.camera = camera;
        }

        public virtual void Update()
        {
            camera.position = Vector2.Lerp(camera.position, PointToFollow, blendFactor);
        }
    }
}
