using Arenar.Services.LevelsService;
using UnityEngine;
using Zenject;


namespace Arenar.Character
{
    public class SGTargetAttackState : AIState
    {
        private ICharacterMovementComponent _characterMovementComponent;
        private ICharacterAggressionComponent _characterAggressionComponent;

        private ICharacterEntity _playerCharacterEntity;

        private ICharacterEntity _character;
        private FirearmWeapon _weapon;
        private WeaponInventoryItemData _weaponInventoryData;
        private ShootingGalleryLevelInfoCollection _shootingGalleryLevelInfoCollection;
        
        
        private ICharacterMovementComponent CharacterMovementComponent
        {
            get
            {
                if (_characterMovementComponent == null)
                    _character.TryGetCharacterComponent(out _characterMovementComponent);
                return _characterMovementComponent;
            }
        }

        private ICharacterAggressionComponent CharacterAggressionComponent
        {
            get
            {
                if (_characterAggressionComponent == null)
                    _character.TryGetCharacterComponent(out _characterAggressionComponent);
                return _characterAggressionComponent;
            }
        }


        [Inject]
        private void Construct(ICharacterEntity character,
            ICharacterDataStorage<SGTargetWeaponDataStorage> weaponDataStorage,
            ShootingGalleryLevelInfoCollection shootingGalleryLevelInfoCollection)
        {
            _character = character;
            _weapon = weaponDataStorage.Data.Weapon;
            _weaponInventoryData = weaponDataStorage.Data.WeaponInventoryData;
            _shootingGalleryLevelInfoCollection = shootingGalleryLevelInfoCollection;
        }
        
        public override void DeInitialize()
        {
            
        }

        public override void OnStateBegin()
        {
            _playerCharacterEntity = CharacterAggressionComponent.AggressionTarget;
        }

        public override void OnStateSyncUpdate()
        {
            if (_playerCharacterEntity == null)
                return;
            Vector3 direction = _playerCharacterEntity.CharacterTransform.position - _character.CharacterTransform.position;
            CharacterMovementComponent.Move(Vector3.zero);
            CharacterMovementComponent.Rotation(direction);

            if (direction.magnitude < 5.0f)
            {
                _weapon.MakeShot(_character.CharacterTransform.forward, true);
            }
        }

        public override void OnStateAsyncUpdate()
        {
            
        }

        public override void OnStateEnd()
        {
            
        }
    }
}