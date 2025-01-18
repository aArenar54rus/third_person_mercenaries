using Arenar.Items;
using Arenar.Services.LevelsService;
using UnityEngine;
using Zenject;


namespace Arenar.Character
{
    public class SGTargetAttackState : AIState
    {
        private ICharacterMovementComponent _characterMovementComponent;
        private ICharacterAggressionComponent _characterAggressionComponent;

        private ICharacterEntity _AggressionTargetCharacter;

        private ICharacterEntity _character;
        private FirearmWeapon _weapon;
        private WeaponInventoryItemData _weaponInventoryData;
        private EffectsSpawner _projectileSpawner;
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
            ShootingGalleryLevelInfoCollection shootingGalleryLevelInfoCollection,
            EffectsSpawner projectileSpawner)
        {
            _character = character;
            _weapon = weaponDataStorage.Data.Weapon;
            _weaponInventoryData = weaponDataStorage.Data.WeaponInventoryData;
            _shootingGalleryLevelInfoCollection = shootingGalleryLevelInfoCollection;
            _projectileSpawner = projectileSpawner;
        }
        
        public override void DeInitialize()
        {
            
        }

        public override void OnStateBegin()
        {
            //_weapon.InitializeItem(_weaponInventoryData);
            //_weapon.effectsSpawner ??= _projectileSpawner;
            _AggressionTargetCharacter = CharacterAggressionComponent.MaxAggressionTarget;
        }

        public override void OnStateSyncUpdate()
        {
            // TODO: remove later
            return;
            if (_AggressionTargetCharacter == null)
                return;
            
            Vector3 direction = (_AggressionTargetCharacter.CharacterTransform.position + Vector3.up) - _character.CharacterTransform.position;
            CharacterMovementComponent.Move(Vector3.zero);
            CharacterMovementComponent.Rotation(direction);
            
            float angle = Vector3.Angle(direction, _character.CharacterTransform.forward);

            if (angle < 5.0f)
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