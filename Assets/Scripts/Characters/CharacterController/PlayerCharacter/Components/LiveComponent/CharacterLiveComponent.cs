using System;
using DG.Tweening;
using UnityEngine;
using Zenject;


namespace Arenar.Character
{
    public class CharacterLiveComponent : ICharacterLiveComponent
    {
        public event Action OnCharacterDie;
        public event Action<ICharacterEntity> OnCharacterGetDamageBy;
        public event Action<int, int> OnCharacterChangeHealthValue;

        
        private PlayerCharacterParametersData playerCharacterParametersData;
        private Transform characterTransform;
        private int healthMax;
        private int health;

        private Tween _deathTween;

        
        public bool IsAlive =>
            health > 0;

        public int Health =>
            health;
        
        public int HealthMax =>
            healthMax;


        [Inject]
        public void Construct(ICharacterDataStorage<CharacterPhysicsDataStorage> characterPhysicsDataStorage,
            PlayerCharacterParametersData playerCharacterParametersData)
        {
            characterTransform = characterPhysicsDataStorage.Data.CharacterTransform;
            this.playerCharacterParametersData = playerCharacterParametersData;
        }

        public void SetDamage(DamageData damageData)
        {
            if (!IsAlive)
                return;
            
            health -= damageData.Damage;
            if (health <= 0)
                SetDeath();
        }

        public void SetAlive()
        {
            health = healthMax;
        }

        public void SetDeath()
        {
            _deathTween = DOVirtual.DelayedCall(1.0f, () => characterTransform.gameObject.SetActive(false));
            health = 0;

            OnCharacterDie?.Invoke();
        }

        public void Initialize()
        {
            _deathTween?.Kill(false);
            healthMax = playerCharacterParametersData.DefaultHealthMax;
            health = healthMax;
        }

        public void DeInitialize()
        {
            _deathTween?.Kill(true);
        }

        public void OnStart()
        {
            SetAlive();
        }
    }
}