using Arenar.Services.DamageNumbersService;
using System;
using Arenar.Services.LevelsService;
using RootMotion.Dynamics;
using Zenject;
using Random = UnityEngine.Random;


namespace Arenar.Character
{
    public class EnemyCharacterLiveComponent : ICharacterLiveComponent
    {
        public event Action<ICharacterEntity> OnCharacterDie;
        public event Action<ICharacterEntity> OnCharacterGetDamageBy;
        public event Action<int, int> OnCharacterChangeHealthValue;


        private CharacterDamageContainer[] _damageContainers;
        private EnemyCharacterDataStorage _enemyCharacterDataStorage;
        private PuppetMaster _puppetMaster;

        private ICharacterEntity _characterEntity;
        private HealthContainer healthContainer;
        
        private IDamageNumbersService _damageNumbers;
        
        
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
                              ICharacterDataStorage<EnemyCharacterDataStorage> enemyCharacterDataStorage,
                              IDamageNumbersService damageNumbers)
        {
            _damageContainers = characterPhysicsDataStorage.Data.DamageContainers;
            _puppetMaster = characterPhysicsDataStorage.Data.PuppetMaster;
            
            _enemyCharacterDataStorage = enemyCharacterDataStorage.Data;
            _characterEntity = characterEntity;

            _damageNumbers = damageNumbers;
        }

        public void SetDamage(DamageData damageData)
        {
            if (!IsAlive)
                return;
            
            float criticalChance = 0.0f;
            if (_enemyCharacterDataStorage.EnemyCharacterParameters.PartDatas.ContainsKey(damageData.BodyPart))
                criticalChance = _enemyCharacterDataStorage.EnemyCharacterParameters.PartDatas[damageData.BodyPart].CriticalChance;

            float random = Random.Range(0, 100);
            bool isCritical = (criticalChance * 100 > random);
            
            var damage = isCritical ? damageData.WeaponDamageWithUpgrades * 2 : damageData.WeaponDamageWithUpgrades;
            HealthContainer.Health -= damage;

            if (_enemyCharacterDataStorage.BodyColliders.ContainsKey(damageData.BodyPart)) {
                _damageNumbers.PlayDamageNumber(
                        damage,
                        _enemyCharacterDataStorage.BodyColliders[damageData.BodyPart].transform,
                        damageData.DamageSetterCharacter.CharacterTransform
                );
            }

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
            _puppetMaster.state = PuppetMaster.State.Dead;
            HealthContainer.Health = 0;
            OnCharacterDie?.Invoke(_characterEntity);
        }

        public void Initialize()
        {
            foreach (var damageContainer in _damageContainers)
                damageContainer.Initialize(_characterEntity);
        }

        public void DeInitialize() {}

        public void OnActivate()
        {
            HealthContainer = new HealthContainer();
            HealthContainer.HealthMax = _enemyCharacterDataStorage.EnemyCharacterParameters.BaseHealth;
            
            SetAlive();

            _puppetMaster.state = PuppetMaster.State.Alive;
        }

        public void OnDeactivate() {}
    }
}