using UnityEngine;


namespace Arenar.Character
{
    public class PlayerComponentCharacterController : ComponentCharacterController,
        ICharacterDataStorage<CharacterAudioDataStorage>,
        ICharacterDataStorage<CharacterVisualDataStorage>,
        ICharacterDataStorage<CharacterAnimatorDataStorage>,
        ICharacterDataStorage<CharacterPhysicsDataStorage>,
        ICharacterDataStorage<CharacterAimAnimationDataStorage>
    {
        [SerializeField] private CharacterAudioDataStorage characterAudioDataStorage;
        [SerializeField] private CharacterVisualDataStorage characterVisualDataStorage;
        [SerializeField] private CharacterAnimatorDataStorage characterAnimatorDataStorage;
        [SerializeField] private CharacterPhysicsDataStorage characterPhysicsDataStorage;
        [SerializeField] private CharacterAimAnimationDataStorage characterAimAnimationDataStorage;
        
        
        CharacterAudioDataStorage ICharacterDataStorage<CharacterAudioDataStorage>.Data =>
            characterAudioDataStorage;

        CharacterVisualDataStorage ICharacterDataStorage<CharacterVisualDataStorage>.Data =>
            characterVisualDataStorage;

        CharacterAnimatorDataStorage ICharacterDataStorage<CharacterAnimatorDataStorage>.Data =>
            characterAnimatorDataStorage;
        
        CharacterPhysicsDataStorage ICharacterDataStorage<CharacterPhysicsDataStorage>.Data =>
            characterPhysicsDataStorage;
        
        CharacterAimAnimationDataStorage ICharacterDataStorage<CharacterAimAnimationDataStorage>.Data =>
            characterAimAnimationDataStorage;
        
        public override Transform CharacterTransform =>
            characterPhysicsDataStorage.CharacterTransform;
        
        public Transform CameraTransform =>
            characterPhysicsDataStorage.CameraTransform;
    }
}