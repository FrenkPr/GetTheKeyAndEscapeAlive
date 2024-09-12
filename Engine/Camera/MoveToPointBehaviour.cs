using Aiv.Fast2D;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownGame
{
    class MoveToPointBehaviour : CameraBehaviour
    {
        protected float timeToMovementEnd;
        protected float movementDuration;
        protected Vector2 cameraStartPosition;

        public MoveToPointBehaviour(Camera camera) : base(camera)
        {

        }

        public virtual void MoveTo(Vector2 point, float movementDuration)
        {
            cameraStartPosition = camera.position;
            PointToFollow = point;
            this.movementDuration = movementDuration;
            timeToMovementEnd = 0;
            blendFactor = 0;
        }

        public override void Update()
        {
            timeToMovementEnd += Game.DeltaTime;

            if (timeToMovementEnd >= movementDuration)
            {
                //animation ended
                timeToMovementEnd = movementDuration;
                CameraMngr.OnMovementEnd();//go to follow target behaviour
            }

            blendFactor = timeToMovementEnd / movementDuration;

            camera.position = Vector2.Lerp(cameraStartPosition, PointToFollow, blendFactor);
        }
    }
}
