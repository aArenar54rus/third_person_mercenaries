using Arenar.Character;
using UnityEngine;


namespace Arenar
{
    public class ShootingGalleryTargetCharacterController : NpcComponentCharacterController, 
        ICharacterDataStorage<CharacterAudioDataStorage>,
        ICharacterDataStorage<CharacterPhysicsDataStorage>
    {
        [SerializeField] private CharacterAudioDataStorage _characterAudioDataStorage;
        [SerializeField] private CharacterPhysicsDataStorage _characterPhysicsDataStorage;
        
        
        public override Transform CharacterTransform =>
            _characterPhysicsDataStorage.CharacterTransform;

        CharacterAudioDataStorage ICharacterDataStorage<CharacterAudioDataStorage>.Data => 
            _characterAudioDataStorage;

        CharacterPhysicsDataStorage ICharacterDataStorage<CharacterPhysicsDataStorage>.Data =>
            _characterPhysicsDataStorage;
    }
}