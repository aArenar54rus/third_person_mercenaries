using System;
using Arenar.Services.LevelsService;
using Arenar.Services.SaveAndLoad;
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

        private ICharacterEntity _playerEntity;
        private ILevelsService _levelsService;
        
        
        public bool IsAlive => HealthContainer.Health > 0;
        public HealthContainer HealthContainer { get; set; }


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
            
            _levelsService.CurrentLevelContext.GettedDamage += damageData.WeaponDamageWithUpgrades;
            HealthContainer.Health -= damageData.WeaponDamageWithUpgrades;
            
            OnCharacterChangeHealthValue?.Invoke(HealthContainer.Health, HealthContainer.HealthMax);
            if (HealthContainer.Health <= 0)
                SetDeath();
        }

        public void SetAlive()
        {
            HealthContainer.Health = HealthContainer.HealthMax;
            OnCharacterChangeHealthValue?.Invoke(HealthContainer.Health, HealthContainer.HealthMax);
        }

        public void SetDeath()
        {
            HealthContainer.Health = 0;
            _levelsService.CurrentLevelContext.PlayerDeath++;

            OnCharacterDie?.Invoke(_playerEntity);
        }

        public void Initialize()
        {
            HealthContainer = new HealthContainer()
            {
                HealthMax = playerCharacterParametersData.DefaultHealthMax,
                Health = playerCharacterParametersData.DefaultHealthMax,
            };
        }

        public void DeInitialize() {}

        public void OnActivate()
        {
            int playerLevel = preferenceManager.LoadValue<PlayerSaveDelegate>().playerCharacterLevel;
            HealthContainer.HealthMax = playerCharacterParametersData.DefaultHealthMax + playerCharacterParametersData.LevelHealthAdded * playerLevel;
            HealthContainer.Health = HealthContainer.HealthMax;
            SetAlive();
        }

        public void OnDeactivate() {}
    }
}