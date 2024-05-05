using System;
using Arenar.Services.LevelsService;
using Arenar.Services.SaveAndLoad;
using DG.Tweening;
using TakeTop.PreferenceSystem;
using UnityEngine;
using Zenject;


namespace Arenar.Character
{
    public class CharacterLiveComponent : ICharacterLiveComponent
    {
        public event Action<ICharacterEntity> OnCharacterDie;
        public event Action<ICharacterEntity> OnCharacterGetDamageBy;
        public event Action<int, int> OnCharacterChangeHealthValue;

        
        private PlayerCharacterParametersData playerCharacterParametersData;
        private IPreferenceManager preferenceManager;
        private Transform characterTransform;
        private int healthMax;
        private int health;

        private ICharacterEntity _playerEntity;
        private ILevelsService _levelsService;

        private Tween _deathTween;

        
        public bool IsAlive =>
            health > 0;

        public int Health =>
            health;
        
        public int HealthMax =>
            healthMax;


        [Inject]
        public void Construct(ICharacterEntity playerEntity,
            ICharacterDataStorage<CharacterPhysicsDataStorage> characterPhysicsDataStorage,
            ILevelsService levelsService,
            IPreferenceManager preferenceManager,
            PlayerCharacterParametersData playerCharacterParametersData)
        {
            characterTransform = characterPhysicsDataStorage.Data.CharacterTransform;
            this.playerCharacterParametersData = playerCharacterParametersData;
            this.preferenceManager = preferenceManager;
            _levelsService = levelsService;
            _playerEntity = playerEntity;
        }

        public void SetDamage(DamageData damageData)
        {
            if (!IsAlive)
                return;
            
            _levelsService.CurrentLevelContext.GettedDamage += damageData.Damage;
            health -= damageData.Damage;
            
            OnCharacterChangeHealthValue?.Invoke(health, healthMax);
            if (health <= 0)
                SetDeath();
        }

        public void SetAlive()
        {
            health = healthMax;
            OnCharacterChangeHealthValue?.Invoke(health, healthMax);
        }

        public void SetDeath()
        {
            _deathTween = DOVirtual.DelayedCall(1.0f, () =>
            {
                characterTransform.gameObject.SetActive(false);
                _levelsService.CompleteLevel();
            });
            health = 0;
            _levelsService.CurrentLevelContext.PlayerDeath++;

            OnCharacterDie?.Invoke(_playerEntity);
        }

        public void Initialize()
        {
            _deathTween?.Kill(false);
        }

        public void DeInitialize()
        {
            _deathTween?.Kill(true);
        }

        public void OnActivate()
        {
            int playerLevel = preferenceManager.LoadValue<PlayerSaveDelegate>().playerCharacterLevel;
            healthMax = playerCharacterParametersData.DefaultHealthMax + playerCharacterParametersData.LevelHealthAdded * playerLevel;
            health = healthMax;
            SetAlive();
        }

        public void OnDeactivate()
        {
            _deathTween?.Kill(true);
        }
    }
}