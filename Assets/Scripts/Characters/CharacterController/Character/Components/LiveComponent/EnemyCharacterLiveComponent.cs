using System;
using Arenar.Services.LevelsService;
using RootMotion.Dynamics;
using Zenject;

namespace Arenar.Character
{
    public class EnemyCharacterLiveComponent : ICharacterLiveComponent
    {
        public event Action<ICharacterEntity> OnCharacterDie;
        public event Action<ICharacterEntity> OnCharacterGetDamageBy;
        public event Action<int, int> OnCharacterChangeHealthValue;


        private CharacterDamageContainer[] damageContainers;
        private EnemyCharacterDataStorage enemyCharacterDataStorage;
        private PuppetMaster puppetMaster;

        private ICharacterEntity characterEntity;
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
                              ILevelsService levelsService,
                              ICharacterDataStorage<CharacterPhysicsDataStorage> characterPhysicsDataStorage,
                              ICharacterDataStorage<EnemyCharacterDataStorage> enemyCharacterDataStorage)
        {
            damageContainers = characterPhysicsDataStorage.Data.DamageContainers;
            puppetMaster = characterPhysicsDataStorage.Data.PuppetMaster;
            
            this.enemyCharacterDataStorage = enemyCharacterDataStorage.Data;
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
            HealthContainer = new HealthContainer();
            HealthContainer.HealthMax = enemyCharacterDataStorage.EnemyCharacterParameters.BaseHealth;
            
            SetAlive();

            puppetMaster.state = PuppetMaster.State.Alive;
        }

        public void OnDeactivate() {}
    }
}