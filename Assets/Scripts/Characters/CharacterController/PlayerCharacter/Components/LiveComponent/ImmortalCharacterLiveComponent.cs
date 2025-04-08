using System;


namespace Arenar.Character
{
    public class ImmortalCharacterLiveComponent : ICharacterLiveComponent
    {
        public event Action<ICharacterEntity> OnCharacterDie;
        public event Action<ICharacterEntity> OnCharacterGetDamageBy;
        public event Action<int, int> OnCharacterChangeHealthValue;
        
        
        public HealthContainer HealthContainer { get; set; }
        public bool IsAlive { get; private set; }


        public void Initialize() { }

        public void DeInitialize() { }

        public void OnActivate()
        {
            IsAlive = true;
            
            HealthContainer = new HealthContainer();
            HealthContainer.HealthMax = HealthContainer.Health = 1;
        }
        
        public void OnDeactivate() { }

        public void SetDamage(DamageData damageData)
        {
            OnCharacterChangeHealthValue?.Invoke(HealthContainer.Health, HealthContainer.HealthMax);
        }

        public void SetAlive() {}

        public void SetDeath() {}
    }
}