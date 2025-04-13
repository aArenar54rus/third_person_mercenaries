using System;
using Arenar.Services.DamageNumbersService;
using Arenar.Services.LevelsService;
using DG.Tweening;
using UnityEngine;
using Zenject;


namespace Arenar.Character
{
    public class SGTargetLiveComponent : ICharacterLiveComponent
    {
        public event Action<ICharacterEntity> OnCharacterDie;
        public event Action<ICharacterEntity> OnCharacterGetDamageBy;
        public event Action<int, int> OnCharacterChangeHealthValue;
        
        private ShootingGalleryTargetCharacterController _character;
        private EnemyCharacterParameters enemyCharacterParameters;
        
        private Transform _characterTransform;
        private Rigidbody _characterRigidbody;
        private EffectsSpawner _effectsSpawner;

        private ILevelsService _levelsService;
        private IDamageNumbersService _damageNumbersService;
        
        private Tween _deathTween;
        
        
        public HealthContainer HealthContainer { get; set; }
        public bool IsAlive => HealthContainer.Health > 0;
        
        
        [Inject]
        public void Construct(ICharacterEntity characterEntity,
            ICharacterDataStorage<SGTargetPhysicalDataStorage> characterPhysicsDataStorage,
            ICharacterDataStorage<EnemyCharacterDataStorage> enemyCharacterDataStorage,
            ILevelsService levelsService,
            EffectsSpawner effectsSpawner,
            IDamageNumbersService damageNumbersService)
        {
            _character = (ShootingGalleryTargetCharacterController)characterEntity;
            _characterTransform = characterPhysicsDataStorage.Data.CharacterTransform;
            _characterRigidbody = characterPhysicsDataStorage.Data.CharacterModelRigidbody;
            _levelsService = levelsService;
            enemyCharacterParameters = enemyCharacterDataStorage.Data.EnemyCharacterParameters;
            _effectsSpawner = effectsSpawner;
            _damageNumbersService = damageNumbersService;
        }
        
        public void Initialize()
        {

        }

        public void DeInitialize()
        {
            _deathTween?.Kill(false);
        }

        public void OnActivate()
        {
            _deathTween?.Kill(false);

            HealthContainer = new HealthContainer();
            HealthContainer.HealthMax = enemyCharacterParameters.BaseHealth
                + enemyCharacterParameters.AddedHealthByLvl
                * (_character.CharacterLevel - 1);
            HealthContainer.Health = HealthContainer.HealthMax;
            SetAlive();
        }

        public void OnDeactivate()
        {
            _deathTween?.Kill(false);
        }

        public void SetDamage(DamageData damageData)
        {
            if (!IsAlive)
                return;
            
            if (damageData.PhysicalMight != Vector3.zero)
                _characterRigidbody.AddForce(damageData.PhysicalMight, ForceMode.Impulse);

            _levelsService.CurrentLevelContext.SettedDamage += damageData.WeaponDamageWithUpgrades;
            HealthContainer.Health -= damageData.WeaponDamageWithUpgrades;
            OnCharacterChangeHealthValue?.Invoke(HealthContainer.Health, HealthContainer.HealthMax);
            
            if (damageData.DamageSetterCharacter != null)
                _damageNumbersService.PlayDamageNumber(damageData.WeaponDamageWithUpgrades, _characterRigidbody.transform, damageData.DamageSetterCharacter.CharacterTransform);
            OnCharacterGetDamageBy?.Invoke(damageData.DamageSetterCharacter);
            
            if (HealthContainer.Health <= 0)
                SetDeath();
        }

        public void SetAlive()
        {
            HealthContainer.Health = HealthContainer.HealthMax;
            _characterRigidbody.velocity = Vector3.zero;
            _characterRigidbody.useGravity = false;
        }

        public void SetDeath()
        {
            _characterRigidbody.useGravity = true;
            HealthContainer.Health = 0;
            _levelsService.CurrentLevelContext.CurrentTargetCount++;
            OnCharacterDie?.Invoke(_character);
            
            _deathTween = DOVirtual.DelayedCall(1.0f, () =>
            {
                var effect = _effectsSpawner.GetEffect(EffectType.RobotBlow);
                effect.gameObject.SetActive(true);
                effect.transform.position = _characterTransform.position;
                effect.Play();
                
                _characterTransform.gameObject.SetActive(false);
            });
        }
    }
}