using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiv.Fast2D;
using OpenTK;

namespace TopDownGame
{
    struct CameraLimits
    {
        public float MaxX;
        public float MinX;
        public float MaxY;
        public float MinY;

        public CameraLimits(float maxX, float minX, float maxY, float minY)
        {
            MaxX = maxX;
            MinX = minX;
            MaxY = maxY;
            MinY = minY;
        }
    }

    static class CameraMngr
    {
        public static Camera MainCamera;
        private static Dictionary<string, Tuple<Camera, float>> subCameras;

        private static CameraBehaviour[] behaviours;
        public static CameraBehaviour CurrentBehaviour { get; private set; }

        public static CameraLimits CameraLimits;
        public static float HalfDiagonalSquared { get => MainCamera.pivot.LengthSquared; }

        public static void Init(GameObject target, CameraLimits cameraLimits)
        {
            MainCamera = new Camera(Game.OrthoHalfWidth, Game.OrthoHalfHeight);
            MainCamera.pivot = Game.ScreenCenter;

            CameraLimits = cameraLimits;

            subCameras = new Dictionary<string, Tuple<Camera, float>>();

            behaviours = new CameraBehaviour[(int)CameraBehaviourType.Length];

            behaviours[(int)CameraBehaviourType.FollowTarget] = new FollowTargetBehaviour(MainCamera, target);
            behaviours[(int)CameraBehaviourType.FollowPoint] = new FollowPointBehaviour(MainCamera, Vector2.Zero);
            behaviours[(int)CameraBehaviourType.MoveToPoint] = new MoveToPointBehaviour(MainCamera);

            CurrentBehaviour = behaviours[(int)CameraBehaviourType.FollowTarget];
        }

        public static void AddCamera(string cameraName, Camera camera = null, float cameraSpeed = 0)
        {
            if (camera == null)
            {
                camera = new Camera(MainCamera.position.X, MainCamera.position.Y);
                camera.pivot = MainCamera.pivot;
            }

            subCameras[cameraName] = new Tuple<Camera, float>(camera, cameraSpeed);
        }

        public static Camera GetCamera(string cameraName)
        {
            if (subCameras != null && subCameras.ContainsKey(cameraName))
            {
                return subCameras[cameraName].Item1;
            }

            return null;
        }

        public static float GetCameraSpeed(string cameraName)
        {
            if (subCameras != null && subCameras.ContainsKey(cameraName))
            {
                return subCameras[cameraName].Item2;
            }

            return 0;
        }

        public static void SetTarget(GameObject target, bool changeBehaviour = true)
        {
            FollowTargetBehaviour followTargetBehaviour = (FollowTargetBehaviour)behaviours[(int)CameraBehaviourType.FollowTarget];
            followTargetBehaviour.Target = target;

            if (changeBehaviour)
            {
                CurrentBehaviour = followTargetBehaviour;
            }
        }

        public static void SetPointToFollow(Vector2 point, bool changeBehaviour = true)
        {
            FollowPointBehaviour followPointBehaviour = (FollowPointBehaviour)behaviours[(int)CameraBehaviourType.FollowPoint];
            followPointBehaviour.SetPointToFollow(point);

            if (changeBehaviour)
            {
                CurrentBehaviour = followPointBehaviour;
            }
        }

        public static void MoveTo(Vector2 point, float time)
        {
            CurrentBehaviour = behaviours[(int)CameraBehaviourType.MoveToPoint];
            ((MoveToPointBehaviour)CurrentBehaviour).MoveTo(point, time);
        }

        public static void OnMovementEnd()
        {
            CurrentBehaviour = behaviours[(int)CameraBehaviourType.FollowTarget];
        }

        public static void Update()
        {
            Vector2 oldCameraPos = MainCamera.position;

            CurrentBehaviour.Update();
            FixMainCameraOutOfBounds();

            Vector2 cameraDelta = MainCamera.position - oldCameraPos;

            UpdateSubCameras(cameraDelta);
        }

        private static void UpdateSubCameras(Vector2 cameraDelta)
        {
            if (cameraDelta != Vector2.Zero)
            {
                //camera moved
                foreach (var camera in subCameras)
                {
                    camera.Value.Item1.position += cameraDelta * camera.Value.Item2;  //camera position += delta * cameraSpeed
                }
            }
        }

        public static void OnChangeWindowSize()
        {
            MainCamera.pivot = Game.ScreenCenter;

            foreach (Tuple<Camera, float> subCamera in subCameras.Values)
            {
                if (subCamera.Item2 != 0)  //if it's not the GUI camera
                {
                    subCamera.Item1.pivot = MainCamera.pivot;
                }
            }

            if (CurrentBehaviour is FollowTargetBehaviour behaviour && behaviour.Target != null)
            {
                MainCamera.position = behaviour.Target.Position;
            }
        }

        private static void FixMainCameraOutOfBounds()
        {
            MainCamera.position.X = MathHelper.Clamp(MainCamera.position.X, CameraLimits.MinX + MainCamera.pivot.X, CameraLimits.MaxX - MainCamera.pivot.X);
            MainCamera.position.Y = MathHelper.Clamp(MainCamera.position.Y, CameraLimits.MinY + MainCamera.pivot.Y, CameraLimits.MaxY - MainCamera.pivot.Y);
        }

        public static void Clear()
        {
            MainCamera = null;
            subCameras.Clear();

            for (int i = 0; i < behaviours.Length; i++)
            {
                behaviours[i] = null;
            }
        }
    }
}
