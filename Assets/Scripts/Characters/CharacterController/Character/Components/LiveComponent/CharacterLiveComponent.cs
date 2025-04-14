using System;
using Arenar.Services.LevelsService;
using Arenar.Services.SaveAndLoad;
using RootMotion.Dynamics;
using Arenar.PreferenceSystem;
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


        private CharacterDamageContainer[] damageContainers;
        private PlayerCharacterParametersData playerCharacterParametersData;
        private PuppetMaster puppetMaster;
        
        private IPreferenceManager preferenceManager;
        private Transform characterTransform;

        private ICharacterEntity characterEntity;
        private ILevelsService levelsService;

        private HealthContainer healthContainer;
        
        
        public bool IsAlive => HealthContainer.Health > 0;
        public HealthContainer HealthContainer
        {
            get => healthContainer;
            set
            {
                healthContainer = value;
                OnCharacterChangeHealthValue?.Invoke(HealthContainer.Health, HealthContainer.HealthMax);
            }
        }


        [Inject]
        public void Construct(ICharacterEntity characterEntity,
                              ICharacterDataStorage<CharacterPhysicsDataStorage> characterPhysicsDataStorage,
                              ILevelsService levelsService,
                              IPreferenceManager preferenceManager,
                              PlayerCharacterParametersData playerCharacterParametersData)
        {
            damageContainers = characterPhysicsDataStorage.Data.DamageContainers;
            characterTransform = characterPhysicsDataStorage.Data.CharacterTransform;
            puppetMaster = characterPhysicsDataStorage.Data.PuppetMaster;
            
            this.playerCharacterParametersData = playerCharacterParametersData;
            this.preferenceManager = preferenceManager;
            this.levelsService = levelsService;
            this.characterEntity = characterEntity;
        }

        public void SetDamage(DamageData damageData)
        {
            if (!IsAlive)
                return;

            var damage = damageData.isCritical ? damageData.WeaponDamageWithUpgrades * 2 : damageData.WeaponDamageWithUpgrades;
            HealthContainer.Health -= damage;
            
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
            puppetMaster.state = PuppetMaster.State.Dead;
            HealthContainer.Health = 0;
            //levelsService.CurrentLevelContext.PlayerDeath++;

            OnCharacterDie?.Invoke(characterEntity);
        }

        public void Initialize()
        {
            foreach (var damageContainer in damageContainers)
                damageContainer.Initialize(characterEntity);
        }

        public void DeInitialize() {}

        public void OnActivate()
        {
            int playerLevel = preferenceManager.LoadValue<PlayerSaveDelegate>().playerCharacterLevel;
            
            HealthContainer = new HealthContainer();
            HealthContainer.HealthMax = playerCharacterParametersData.DefaultHealthMax /*+ playerCharacterParametersData.LevelHealthAdded * playerLevel*/;
            
            SetAlive();

            puppetMaster.state = PuppetMaster.State.Alive;
        }

        public void OnDeactivate() {}
    }
}