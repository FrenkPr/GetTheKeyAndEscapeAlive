using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiv.Audio;

namespace TopDownGame
{
    static class SoundMngr
    {
        private static Dictionary<string, AudioClip> sceneClips;
        private static Dictionary<string, AudioClip> globalClips;

        static SoundMngr()
        {
            sceneClips = new Dictionary<string, AudioClip>();
            globalClips = new Dictionary<string, AudioClip>();
        }

        public static AudioClip AddClip(string name, string path, bool globalClip = false)
        {
            AudioClip c = new AudioClip(path);

            if (c != null)
            {
                if (globalClip)
                {
                    globalClips[name] = c;
                }

                else
                {
                    sceneClips[name] = c;
                }
            }

            return c;
        }

        public static AudioClip GetClip(string name)
        {
            AudioClip c = null;

            if (globalClips.ContainsKey(name))
            {
                c = globalClips[name];
            }

            else if (sceneClips.ContainsKey(name))
            {
                c = sceneClips[name];
            }

            return c;
        }

        public static void ClearAll(bool clearGlobalClips = false)
        {
            sceneClips.Clear();
            sceneClips = null;

            if (clearGlobalClips)
            {
                globalClips.Clear();
                globalClips = null;
            }
        }
    }
}
