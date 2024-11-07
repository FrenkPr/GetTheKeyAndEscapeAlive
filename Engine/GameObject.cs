using Aiv.Fast2D;
using OpenTK;
using System;

namespace TopDownGame
{
    class GameObject : IDrawable, IUpdatable, ICloneable
    {
        protected Texture texture;
        protected string textureId;

        protected Vector4 color;

        public Sprite Sprite { get; protected set; }
        public RigidBody RigidBody;
        public bool IsActive;

        public virtual Vector2 Position { get { return Sprite.position; } set { Sprite.position = value; } }
        public virtual float X { get { return Sprite.position.X; } set { Sprite.position.X = value; } }
        public virtual float Y { get { return Sprite.position.Y; } set { Sprite.position.Y = value; } }

        public Vector2 Forward;
        public Vector2 LastForward;

        public float Width { get { return Sprite.Width; } }
        public float Height { get { return Sprite.Height; } }
        public float HalfWidth { get { return Width * 0.5f; } }
        public float HalfHeight { get { return Height * 0.5f; } }
        public float PixelsWidth { get; }
        public float PixelsHeight { get; }

        public DrawLayer DrawLayer { get; private set; }
        private DrawingType drawingType;
        private UpdatingType updatingType;

        public int NumFrames { get; }
        public int CurrentFrame;

        public GameObject(string textureId, int numFrames = 1, float spriteWidth = 0, float spriteHeight = 0, DrawLayer dLayer = DrawLayer.Playground, DrawingType dType = DrawingType.Play, UpdatingType uType = UpdatingType.Play, Vector4? color = null)
        {
            DrawLayer = dLayer;

            if (textureId != "")
            {
                this.textureId = textureId;
                texture = TextureMngr.GetTexture(this.textureId);

                NumFrames = numFrames;
                CurrentFrame = 0;

                spriteWidth = spriteWidth <= 0 ? texture.Width : spriteWidth;
                spriteHeight = spriteHeight <= 0 ? texture.Height : spriteHeight;

                if (numFrames > 1)
                {
                    spriteWidth /= numFrames;
                }
            }
            
            this.color = color == null ? -Vector4.One : (Vector4)color;

            PixelsWidth = spriteWidth;
            PixelsHeight = spriteHeight;

            spriteWidth = Game.PixelsToUnits(spriteWidth);
            spriteHeight = Game.PixelsToUnits(spriteHeight);

            Sprite = new Sprite(spriteWidth, spriteHeight);
            Sprite.pivot = new Vector2(spriteWidth * 0.5f, spriteHeight * 0.5f);

            drawingType = dType;
            updatingType = uType;

            if (textureId != "" || this.color != -Vector4.One)
            {
                DrawMngr.Add(this, drawingType);
            }

            UpdateMngr.Add(this, updatingType);
        }

        public virtual void OnCollision(CollisionInfo collisionInfo)
        {

        }

        public virtual void Update()
        {

        }

        public virtual void Draw()
        {
            if (IsActive)
            {
                if (texture != null)
                {
                    if (NumFrames <= 1)
                    {
                        Sprite.DrawTexture(texture);
                    }

                    else
                    {
                        Sprite.DrawTexture(texture, (int)(PixelsWidth * CurrentFrame), 0, (int)PixelsWidth, (int)PixelsHeight);
                    }
                }

                else if (color != -Vector4.One)
                {
                    Sprite.DrawColor((int)color.X, (int)color.Y, (int)color.Z, (int)color.W);
                }
            }
        }

        public object Clone()
        {
            GameObject gameObjectClone = new GameObject(textureId, NumFrames, PixelsWidth, PixelsHeight, DrawLayer, drawingType, updatingType);
            gameObjectClone.Sprite.pivot = Sprite.pivot;

            return gameObjectClone;
        }
    }
}
