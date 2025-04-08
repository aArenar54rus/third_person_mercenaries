using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;


namespace Arenar.Character
{
    [RequireComponent(typeof(Collider))]
    public class DamageLocatorContainer : MonoBehaviour
    {
        public const int MIN_VOLUME = -80;
        public const int MAX_VOLUME = 0;
        public const string SOUNDS_VOLUME = "SoundsVolume";
        
        [SerializeField]
        private ParticleSystem particles;

        [SerializeField]
        private Slider slider;
        
        [SerializeField]
        private Collider bodyCollider;

        public AudioMixerGroup group;


        public void ChangeVolume(float t)
        {
            //group.audioMixer.SetFloat(SOUNDS_VOLUME, Mathf.Lerp(MIN_VOLUME, MAX_VOLUME, volume));
            group.audioMixer.SetFloat("MasterVolume", Mathf.Lerp(MIN_VOLUME, MAX_VOLUME, t));
            
            
            
            particles.Play();
            particles.Stop();
            particles.Pause();
            particles.transform.position = Vector3.zero;

            Debug.LogError(slider.value);
        }


        private void Update()
        {
            Debug.LogError(slider.value);
            slider.onValueChanged.AddListener(
                delegate
                {
                    ChangeVolume(slider.value);
                });
        }
    }
}