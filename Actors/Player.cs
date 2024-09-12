using Aiv.Fast2D;
using OpenTK;
using System;
using System.Collections.Generic;

namespace TopDownGame
{
    class Player : Actor
    {
        private bool isMouseLeftPressed;
        private bool isOpenDoorKeyPressed;
        public int Id { get; private set; }

        public Player(int id) : base("player", new Vector2(2), width: 32, height: 32)
        {
            Id = id;

            IsActive = true;

            RigidBody.Type = RigidBodyType.Player;
            RigidBody.IsMovableViaInput = true;

            DrawMngr.Add(this);
            //DebugMngr.AddItem(RigidBody.Collider);
        }

        public void Input()
        {
            if (!IsActive)
            {
                return;
            }

            GoToMousePosition();
            OpenDoor();
        }

        private void GoToMousePosition()
        {
            if (MouseController.IsMouseButtonPressed(MouseButton.LeftClick))
            {
                if (!isMouseLeftPressed && !Game.CurrentScene.PauseMenu.PlayerInputsLocked && Game.Window.MouseX >= 0 && Game.Window.MouseX < Game.OrthoWidth &&
                    Game.Window.MouseY >= 0 && Game.Window.MouseY < Game.OrthoHeight)
                {
                    RigidBody.Velocity = RigidBody.MoveSpeed;

                    Vector2 mouseAbsolutePosition = CameraMngr.MainCamera.position - CameraMngr.MainCamera.pivot + Game.MousePosition;

                    //player path
                    List<Node> path = Game.CurrentScene.TiledMap.PathFindingMap.GetPath(X, Y, mouseAbsolutePosition.X, mouseAbsolutePosition.Y);
                    Agent.SetPath(path);

                    if (Agent.Path.Count > 0)
                    {
                        LastForward = Vector2.Zero;
                    }

                    isMouseLeftPressed = true;
                }
            }

            else if (isMouseLeftPressed || Game.CurrentScene.PauseMenu.PlayerInputsLocked)
            {
                isMouseLeftPressed = false;
                Game.CurrentScene.PauseMenu.PlayerInputsLocked = false;
            }
        }

        private void OpenDoor()
        {
            if (Game.KeyboardCtrl.OnKeyPressed(KeyCodeType.Interact))
            {
                if (!isOpenDoorKeyPressed && !Game.CurrentScene.PauseMenu.PlayerInputsLocked && TileNextSceneLocked.TileNextSceneLockedCollidedWithPlayer != null && TileNextSceneLocked.TileNextSceneLockedCollidedWithPlayer.PlayerGotTheKey)
                {
                    Vector2 nextSceneLockedPos = TileNextSceneLocked.TileNextSceneLockedCollidedWithPlayer.Position;
                    float nextSceneLockedHalfWidth = TileNextSceneLocked.TileNextSceneLockedCollidedWithPlayer.HalfWidth;
                    float nextSceneLockedHalfHeight = TileNextSceneLocked.TileNextSceneLockedCollidedWithPlayer.HalfHeight;

                    TileNextSceneLocked.TileNextSceneLockedCollidedWithPlayer.UnlockNextSceneLocked();
                    Game.CurrentScene.TiledMap.PathFindingMap.AddNode(nextSceneLockedPos.X + nextSceneLockedHalfWidth, nextSceneLockedPos.Y + nextSceneLockedHalfHeight);
                    isOpenDoorKeyPressed = true;
                }
            }

            else if (isOpenDoorKeyPressed || Game.CurrentScene.PauseMenu.PlayerInputsLocked)
            {
                isOpenDoorKeyPressed = false;
                Game.CurrentScene.PauseMenu.PlayerInputsLocked = false;
            }
        }

        public override void OnDie()
        {
            IsAlive = false;
            IsActive = false;
            Game.CurrentScene.GameEnded = true;
            DrawMngr.Remove(this);
        }

        public override void Update()
        {
            base.Update();

            //Console.WriteLine("Player position: " + Position);
            //Game.CurrentScene.TiledMap.PathfindingMap.GetNodeAtPos(Position.X, Position.Y);
        }
    }
}
