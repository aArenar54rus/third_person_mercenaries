namespace Arenar.Character
{
    public interface IStunCharacterComponent : ICharacterComponent
    {
        bool IsStunned { get; }
        
        
        void AddStunPoints(int stunPoints);
    }
}