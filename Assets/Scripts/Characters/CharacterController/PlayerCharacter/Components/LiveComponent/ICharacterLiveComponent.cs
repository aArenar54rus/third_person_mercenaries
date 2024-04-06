using System;


namespace Arenar.Character
{
    public interface ICharacterLiveComponent : ICharacterComponent
    {
        event Action OnCharacterDie; 
        event Action<ICharacterEntity> OnCharacterGetDamageBy; 
        event Action<int, int> OnCharacterChangeHealthValue; 
        
        
        bool IsAlive { get; }
        
        int Health { get; }
        
        int HealthMax { get; }


        void SetDamage(DamageData damageData);
        
        void SetAlive();
        
        void SetDeath();
    }
}