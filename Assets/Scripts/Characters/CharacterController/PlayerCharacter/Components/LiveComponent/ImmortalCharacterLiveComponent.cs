using System;


namespace Arenar.Character
{
    public class ImmortalCharacterLiveComponent : ICharacterLiveComponent
    {
        public event Action<ICharacterEntity> OnCharacterDie;
        public event Action<ICharacterEntity> OnCharacterGetDamageBy;
        public event Action<int, int> OnCharacterChangeHealthValue;


        public bool IsAlive { get; private set; }
        public int Health { get; private set; }
        public int HealthMax { get; private set; }


        public void Initialize()
        {
            IsAlive = true;
            HealthMax = Health = 1;
        }

        public void DeInitialize() { }

        public void OnStart() { }

        public void SetDamage(DamageData damageData)
        {
            OnCharacterChangeHealthValue?.Invoke(Health, HealthMax);
        }

        public void SetAlive() {}

        public void SetDeath() {}
    }
}