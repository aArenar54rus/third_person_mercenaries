using JetBrains.Annotations;
using Zenject;


namespace Arenar.Character
{
    public class PlayerCharacterAttackComponent : IAttackComponent, ITickable
    {
        private ICharacterEntity character;
        private TickableManager tickableManager;
        private ItemProjectileSpawner itemProjectileSpawner;

        private ICharacterInputComponent characterInputComponent;
        
        
        [Inject]
        public void Construct(ICharacterEntity character, [CanBeNull] TickableManager tickableManager, ItemProjectileSpawner itemProjectileSpawner)
        {
            this.character = character;
            this.tickableManager = tickableManager;
        }
        
        public void Initialize()
        {
            tickableManager.Add(this);
        }

        public void DeInitialize()
        {
            tickableManager.Remove(this);
        }

        public void OnStart()
        {
            characterInputComponent = character.TryGetCharacterComponent<ICharacterInputComponent>(out bool success);
        }

        public void Tick()
        {
            if (characterInputComponent == null)
                return;
            
            if (characterInputComponent.AimAction)
            {
                CheckMeleeAttack();
            }
            else
            {
                CheckDistanceAttack();
            }
        }

        private void CheckMeleeAttack()
        {
            
        }

        private void CheckDistanceAttack()
        {
            var bullet = itemProjectileSpawner.GetItemProjectile(ItemProjectileType.Bullet);
            //bullet.Initialize();
        }
    }
}