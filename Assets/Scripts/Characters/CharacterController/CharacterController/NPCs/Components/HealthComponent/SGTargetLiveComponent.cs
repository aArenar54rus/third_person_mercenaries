using System;
using DG.Tweening;
using UnityEngine;
using Zenject;


namespace Arenar.Character
{
    public class SGTargetLiveComponent : ICharacterLiveComponent
    {
        public event Action OnCharacterDie;
        public event Action<ICharacterEntity> OnCharacterGetDamageBy;
        public event Action<int, int> OnCharacterChangeHealthValue;


        private ShootingGalleryTargetCharacterController _character;
        private ShootingGalleryTargetParameters _shootingGalleryTargetParameters;
        
        private Transform _characterTransform;
        private Rigidbody _characterRigidbody;
        private EffectsSpawner _effectsSpawner;
        
        private Tween _deathTween;
        
        private int _health;
        
        
        public bool IsAlive => _health > 0;

        public int Health
        {
            get => _health;
            private set
            {
                _health = Mathf.Clamp(value, 0, HealthMax);
                OnCharacterChangeHealthValue?.Invoke(HealthMax, Health);
            }
        }

        public int HealthMax { get; private set; }
        
        
        [Inject]
        public void Construct(ICharacterEntity characterEntity,
            ICharacterDataStorage<SGTargetPhysicalDataStorage> characterPhysicsDataStorage,
            ShootingGalleryTargetParameters shootingGalleryTargetParameters,
            EffectsSpawner effectsSpawner)
        {
            _character = (ShootingGalleryTargetCharacterController)characterEntity;
            _characterTransform = characterPhysicsDataStorage.Data.CharacterTransform;
            _characterRigidbody = characterPhysicsDataStorage.Data.CharacterModelRigidbody;
            _shootingGalleryTargetParameters = shootingGalleryTargetParameters;
            _effectsSpawner = effectsSpawner;
        }
        
        public void Initialize()
        {
            _deathTween?.Kill(false);

            HealthMax = _shootingGalleryTargetParameters.BaseHealth
                        + _shootingGalleryTargetParameters.AddedHealthByLvl * (_character.CharacterLevel - 1);
            Health = HealthMax;
        }

        public void DeInitialize()
        {

        }

        public void OnStart()
        {
            SetAlive();
        }
        
        public void SetDamage(DamageData damageData)
        {
            if (!IsAlive)
                return;
            
            if (damageData.BulletMight != Vector3.zero)
                _characterRigidbody.AddForce(damageData.BulletMight, ForceMode.Impulse);
            
            Health -= damageData.Damage;
            OnCharacterGetDamageBy?.Invoke(damageData.DamageSetterCharacter);
            
            if (Health <= 0)
                SetDeath();
        }

        public void SetAlive()
        {
            Health = HealthMax;
            
            _characterRigidbody.velocity = Vector3.zero;
            _characterRigidbody.useGravity = false;
        }

        public void SetDeath()
        {
            _characterRigidbody.useGravity = true;
            Health = 0;
            OnCharacterDie?.Invoke();
            
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