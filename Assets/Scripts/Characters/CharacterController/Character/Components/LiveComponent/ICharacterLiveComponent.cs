using System;


namespace Arenar.Character
{
    public interface ICharacterLiveComponent : ICharacterComponent
    {
        event Action<ICharacterEntity> OnCharacterDie; 
        event Action<ICharacterEntity> OnCharacterGetDamageBy; 
        event Action<int, int> OnCharacterChangeHealthValue; 
        
        
        HealthContainer HealthContainer { get; set; }
        bool IsAlive { get; }


        void SetDamage(DamageData damageData);
        
        void SetAlive();
        
        void SetDeath();
    }
}