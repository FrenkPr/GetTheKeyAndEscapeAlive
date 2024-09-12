using System;
using System.Collections.Generic;
using OpenTK;

namespace TopDownGame
{
    enum ActorAnimationStateType
    {
        Idle,
        Walk,
        Length
    }

    abstract class Actor : GameObject
    {
        public bool IsAlive;
        public Agent Agent { get; protected set; }
        private Dictionary<Vector2, ActorAnimation>[] animationStates;

        public Actor(string actorName, Vector2 moveSpeed = default, int numFrames = 1, int width = 0, int height = 0) : base("", numFrames, width, height)
        {
            RigidBody = new RigidBody(this, moveSpeed);
            RigidBody.Collider = ColliderFactory.CreateBoxFor(this);

            //DebugMngr.AddItem(RigidBody.Collider);

            Agent = new Agent(this, new Vector4(1, 0, 0, 1));

            animationStates = new Dictionary<Vector2, ActorAnimation>[(int)ActorAnimationStateType.Length];

            //IDLE ACTOR ANIMATIONS
            animationStates[(int)ActorAnimationStateType.Idle] = new Dictionary<Vector2, ActorAnimation>();

            animationStates[(int)ActorAnimationStateType.Idle].Add(new Vector2(0, -1), new ActorAnimation(this, $"{actorName}IdleU", 1));
            animationStates[(int)ActorAnimationStateType.Idle].Add(new Vector2(0, 1), new ActorAnimation(this, $"{actorName}IdleD", 1));
            animationStates[(int)ActorAnimationStateType.Idle].Add(new Vector2(-1, 0), new ActorAnimation(this, $"{actorName}IdleR", 1, true));
            animationStates[(int)ActorAnimationStateType.Idle].Add(new Vector2(1, 0), new ActorAnimation(this, $"{actorName}IdleR", 1));


            //WALK ACTOR ANIMATIONS
            animationStates[(int)ActorAnimationStateType.Walk] = new Dictionary<Vector2, ActorAnimation>();

            animationStates[(int)ActorAnimationStateType.Walk].Add(new Vector2(0, -1), new ActorAnimation(this, $"{actorName}WalkU", 4));
            animationStates[(int)ActorAnimationStateType.Walk].Add(new Vector2(0, 1), new ActorAnimation(this, $"{actorName}WalkD", 4));
            animationStates[(int)ActorAnimationStateType.Walk].Add(new Vector2(-1, 0), new ActorAnimation(this, $"{actorName}WalkR", 4, true));
            animationStates[(int)ActorAnimationStateType.Walk].Add(new Vector2(1, 0), new ActorAnimation(this, $"{actorName}WalkR", 4));


            Forward = new Vector2(0, 1);
            LastForward = new Vector2(0, 1);
        }

        public virtual void OnDie()
        {

        }

        public override void Update()
        {
            Agent.Update();

            if (LastForward != Vector2.Zero)
            {
                animationStates[(int)ActorAnimationStateType.Idle][LastForward].Update();
            }

            else
            {
                animationStates[(int)ActorAnimationStateType.Walk][Forward].Update();
            }
        }

        public override void Draw()
        {
            //Console.WriteLine("FORWARD: " + Forward);

            if (LastForward != Vector2.Zero)
            {
                animationStates[(int)ActorAnimationStateType.Idle][LastForward].Draw();
            }

            else
            {
                animationStates[(int)ActorAnimationStateType.Walk][Forward].Draw();
            }
        }
    }
}
