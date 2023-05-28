using Arenar.AudioSystem;
using UnityEngine;
using Zenject;


namespace CatSimulator.Character
{
    public class CharacterSoundComponent : ICharacterSoundComponent<CharacterSoundComponent.KittySounds>
    {
        public enum KittySounds
        {
            None = 0,
            Footstep = 1,
        }


        private ICharacterEntity characterEntity;
        private AudioController audioController;
        private CharacterAudioDataStorage characterAudioDataStorage;
        private SoundsLibrary soundsLibrary;


        private ICharacterLiveComponent LiveComponent =>
            characterEntity.TryGetCharacterComponent<ICharacterLiveComponent>(out bool success);
        
        
        [Inject]
        public void Construct(ICharacterDataStorage<CharacterAudioDataStorage> characterAudioDataStorage,
            ICharacterEntity characterEntity,
            AudioLibrary audioLibrary)
        {
            this.soundsLibrary = audioLibrary.SoundsLibrary;
            this.characterEntity = characterEntity;
            this.characterAudioDataStorage = characterAudioDataStorage.Data;
        }
        
        public void Initialize()
        {
            audioController = new AudioController(characterAudioDataStorage.AudioSource);
            characterAudioDataStorage.AnimationReactionsController.onFootStep += PlayRandomFootstepSound;
            LiveComponent.OnKittyDie += OnKittyDie;
        }

        public void DeInitialize()
        {
            audioController = null;
            characterAudioDataStorage.AnimationReactionsController.onFootStep -= PlayRandomFootstepSound;
            LiveComponent.OnKittyDie -= OnKittyDie;
        }

        public void OnStart()
        {
            StopSound();
        }

        public void PlaySound(KittySounds soundType)
        {
            switch (soundType)
            {
                case KittySounds.None:
                    return;
                
                case KittySounds.Footstep:
                    audioController.PlaySound(soundsLibrary.GetRandomGroundStepSound(GroundType.Concrete), false);
                    return;
                
                default:
                    Debug.LogError("Not found sound type {soundType} for kitty character.");
                    return;
            }
        }

        public void StopSound() =>
            audioController.StopSound();

        private void OnKittyDie()
        {
            StopSound();
        }

        private void PlayRandomFootstepSound()
        {
            PlaySound(KittySounds.Footstep);
        }
    }
}