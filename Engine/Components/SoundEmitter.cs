using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiv.Audio;

namespace TopDownGame
{
    class SoundEmitter : Component
    {
        public static Dictionary<string, SoundEmitter> GlobalAudioSources {  get; private set; }
        public static Dictionary<string, SoundEmitter> SceneAudioSources {  get; private set; }
        protected AudioSource source;
        protected AudioClip clip;

        public float Volume { get { return source.Volume; } set { source.Volume = value; } }
        public float Pitch { get { return source.Pitch; } set { source.Pitch = value; } }

        public SoundEmitter(GameObject owner, string clipName) : base(owner)
        {
            source = new AudioSource();
            clip = SoundMngr.GetClip(clipName);

            SceneAudioSources[clipName] = this;
        }

        public static void InitAudioSources(bool initGlobalAudioSources = false)
        {
            if (initGlobalAudioSources)
            {
                GlobalAudioSources = new Dictionary<string, SoundEmitter>();
            }

            SceneAudioSources = new Dictionary<string, SoundEmitter>();
        }

        public static void SetAudioVolume(float volume)
        {
            if (GlobalAudioSources != null)
            {
                foreach (var globalAudioSource in GlobalAudioSources)
                {
                    globalAudioSource.Value.source.Volume = volume;
                }
            }

            if (SceneAudioSources != null)
            {
                foreach (var sceneAudioSource in SceneAudioSources)
                {
                    sceneAudioSource.Value.source.Volume = volume;
                } 
            }
        }

        public void Play(bool loop = false)
        {
            source.Play(clip, loop);
        }

        public bool IsPlaying()
        {
            return source.IsPlaying;
        }

        public void RandomizePitch()
        {
            Pitch = RandomGenerator.GetRandomFloat() * 0.4f + 0.8f; //0.8f => 1.2f
        }

        public void Play(float volume, float pitch = 1f, AudioClip clipToPlay = null)
        {
            source.Volume = volume;
            source.Pitch = pitch;
            source.Play(clipToPlay != null ? clipToPlay : clip);
        }

        public static void ClearAudioSources(bool clearGlobalAudioSources = false)
        {
            if (clearGlobalAudioSources)
            {
                GlobalAudioSources.Clear();
                GlobalAudioSources = null; 
            }

            SceneAudioSources.Clear();
            SceneAudioSources = null;
        }
    }
}
