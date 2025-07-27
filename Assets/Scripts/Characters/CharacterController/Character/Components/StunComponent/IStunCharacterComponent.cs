namespace Arenar.Character
{
    public interface IStunCharacterComponent : ICharacterComponent
    {
        bool IsStunned { get; }
        
        
        void AddStunPoints(DamageData damageData);
    }
}