using System;


namespace Arenar.Character
{
    public interface ICharacterLiveComponent : ICharacterComponent
    {
        event Action OnCharacterDie; 
        event Action<int, int> OnCharacterChangeHealthValue; 
        
        
        bool IsAlive { get; }
        
        int Health { get; }
        
        int HealthMax { get; }


        void SetDamage(int damageCount);
        
        void SetAlive();
        
        void SetDeath();
    }
}