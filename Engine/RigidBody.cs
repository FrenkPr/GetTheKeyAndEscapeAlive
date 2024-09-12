using System;
using System.Collections.Generic;
using OpenTK;
using Aiv.Fast2D;

namespace TopDownGame
{
    enum RigidBodyType
    {
        Player = 1,
        SimpleTileObject = 2,
        InfiniteNodeCost = 4,
        TileNextScene = 8,
        TileNextSceneLocked = 16,
        TileLethalObject = 32,
        TileToggleableLethalObject = 64,
        KeyObject = 128,
        TileTypesLength = 6
    }

    class RigidBody
    {
        public GameObject GameObject;
        public Collider Collider;
        public bool IsCollisionAffected;
        public RigidBodyType Type;
        protected uint collisionMask;

        public Vector2 Position { get { return GameObject.Position; } set { GameObject.Position = value; } }

        public Vector2 Velocity;
        public Vector2 MoveSpeed;
        public bool IsMovableViaInput;

        public bool IsActive { get { return GameObject.IsActive; } }

        public RigidBody(GameObject owner, Vector2 moveSpeed)
        {
            GameObject = owner;
            Velocity = moveSpeed;
            MoveSpeed = moveSpeed;
            IsCollisionAffected = true;
            IsMovableViaInput = false;

            PhysicsMngr.Add(this);
        }

        public bool Collides(RigidBody otherBody, ref CollisionInfo collisionInfo)
        {
            return Collider.Collides(otherBody.Collider, ref collisionInfo);
        }

        public void Update()
        {
            if (IsActive && !IsMovableViaInput)
            {
                Position += Velocity * Game.DeltaTime;  //updates game object position
            }
        }

        public void AddCollisionType(RigidBodyType type)
        {
            collisionMask |= (uint)type;
        }

        public bool CollisionTypeMatches(RigidBodyType type)
        {
            return ((uint)type & collisionMask) != 0;
        }
    }
}
