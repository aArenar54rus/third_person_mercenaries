using UnityEngine;
using UnityEngine.Audio;

namespace Arenar.AudioSystem
{
    public interface IAudioSystemManager
    {
        void InitializeVolumes();

        void SetVolume(AudioSystemType type, bool status, float volume);

		AudioMixerGroup GetAudioMixerGroup(AudioSystemType type);

        public AudioSource CreateAudioSource(GameObject audioSourceParent, AudioSystemType type);
    }
}
