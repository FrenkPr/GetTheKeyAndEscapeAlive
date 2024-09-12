using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownGame
{
    class Animation : Component
    {
        private Timer timeToNextFrame;
        private bool isLoopable;

        public Animation(GameObject owner, float fps, bool loop = true) : base(owner)
        {
            float frameDuration = 1 / fps;
            timeToNextFrame = new Timer(frameDuration, frameDuration);
            isLoopable = loop;
        }

        public void Update()
        {
            if (IsEnabled)
            {
                timeToNextFrame.DecTime();

                if (timeToNextFrame.Clock <= 0)
                {
                    GameObject.CurrentFrame++;

                    if (GameObject.CurrentFrame >= GameObject.NumFrames)
                    {
                        Restart();

                        if (!isLoopable)
                        {
                            Stop();
                        }
                    }

                    timeToNextFrame.Reset();
                }
            }
        }

        public void Play()
        {
            GameObject.CurrentFrame = 0;
            IsEnabled = true;
            timeToNextFrame.Reset();
        }

        public void Stop()
        {
            IsEnabled = false;
        }

        public void Resume()
        {
            IsEnabled = true;
        }

        public void Restart()
        {
            Play();
        }
    }
}
