using Arenar.AudioSystem;
using UnityEngine;
using Zenject;


namespace Arenar.Character
{
    public class CharacterSoundComponent : ICharacterSoundComponent<CharacterSoundComponent.HumanoidSounds>
    {
        public enum HumanoidSounds
        {
            None = 0,
            Footstep = 1,
        }


        private ICharacterEntity characterEntity;
        private AudioController audioController;
        private CharacterAudioDataStorage characterAudioDataStorage;
        private SoundsLibrary soundsLibrary;
        private ICharacterLiveComponent liveComponent;
        
        
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
            characterEntity.TryGetCharacterComponent<ICharacterLiveComponent>(out liveComponent);
            audioController = new AudioController(characterAudioDataStorage.AudioSource);
        }

        public void DeInitialize()
        {
            audioController = null;
        }

        public void OnActivate()
        {
            characterAudioDataStorage.AnimationReactionsTriggerController.onFootStep += PlayRandomFootstepSound;
            liveComponent.OnCharacterDie += OnCharacterDie;
            StopSound();
        }

        public void OnDeactivate()
        {
            characterAudioDataStorage.AnimationReactionsTriggerController.onFootStep -= PlayRandomFootstepSound;
            liveComponent.OnCharacterDie -= OnCharacterDie;
            StopSound();
        }

        public void PlaySound(HumanoidSounds soundType)
        {
            switch (soundType)
            {
                case HumanoidSounds.None:
                    return;
                
                case HumanoidSounds.Footstep:
                    audioController.PlaySound(soundsLibrary.GetRandomGroundStepSound(GroundType.Concrete), false);
                    return;
                
                default:
                    Debug.LogError("Not found sound type {soundType} for kitty character.");
                    return;
            }
        }

        public void StopSound() =>
            audioController?.StopSound();

        private void OnCharacterDie(ICharacterEntity characterEntity)
        {
            StopSound();
        }

        private void PlayRandomFootstepSound()
        {
            PlaySound(HumanoidSounds.Footstep);
        }
    }
}