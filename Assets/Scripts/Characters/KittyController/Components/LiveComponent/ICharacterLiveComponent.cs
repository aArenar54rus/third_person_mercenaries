using System;


namespace CatSimulator.Character
{
    public interface ICharacterLiveComponent : ICharacterComponent
    {
        event Action OnKittyDie; 
        
        
        bool IsAlive { get; }
        
        int Health { get; }
        
        int HealthMax { get; }


        void SetDamage(int damageCount);
        
        void SetAlive();
        
        void SetDeath();
    }
}