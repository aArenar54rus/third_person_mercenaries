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
        private FirearmWeaponItemData firearmWeaponData;
        private EffectsSpawner _projectileSpawner;
        private ClearLocationLevelInfoCollection clearLocationLevelInfoCollection;
        
        
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
            ClearLocationLevelInfoCollection clearLocationLevelInfoCollection,
            EffectsSpawner projectileSpawner)
        {
            _character = character;
            _weapon = weaponDataStorage.Data.Weapon;
            firearmWeaponData = weaponDataStorage.Data.FirearmWeaponData;
            this.clearLocationLevelInfoCollection = clearLocationLevelInfoCollection;
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
            CharacterMovementComponent.Move(Vector2.zero, false);
            CharacterMovementComponent.Rotation(direction);
            
            float angle = Vector3.Angle(direction, _character.CharacterTransform.forward);

            if (angle < 5.0f)
            {
                _weapon.MakeShot(_character.CharacterTransform.forward, 0,true);
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