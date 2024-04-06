using UnityEngine;


namespace Arenar.Character
{
    public class ShootingGalleryTargetCharacterController : NpcComponentCharacterController, 
                                                            ICharacterDataStorage<CharacterAudioDataStorage>,
                                                            ICharacterDataStorage<SGTargetPhysicalDataStorage>,
                                                            ICharacterDataStorage<SGTargetWeaponDataStorage>
    {
        [SerializeField] private CharacterAudioDataStorage _characterAudioDataStorage;
        [SerializeField] private SGTargetPhysicalDataStorage _characterPhysicsDataStorage;
        [SerializeField] private SGTargetWeaponDataStorage _characterWeaponDataStorage;


        public override Transform CharacterTransform =>
            _characterPhysicsDataStorage.CharacterTransform;
        
        public int TargetCharacterIndex { get; private set; }
        
        public int CharacterLevel { get; private set; }

        CharacterAudioDataStorage ICharacterDataStorage<CharacterAudioDataStorage>.Data => 
            _characterAudioDataStorage;

        SGTargetPhysicalDataStorage ICharacterDataStorage<SGTargetPhysicalDataStorage>.Data =>
            _characterPhysicsDataStorage;
        
        SGTargetWeaponDataStorage ICharacterDataStorage<SGTargetWeaponDataStorage>.Data =>
            _characterWeaponDataStorage;


        public void InitializeShooterGalleryTarget(int characterIndex, int characterLevel)
        {
            TargetCharacterIndex = characterIndex;
            CharacterLevel = characterLevel;
        }
    }
}