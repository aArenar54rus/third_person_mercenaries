namespace Arenar.Character
{
    public interface ICharacterAggressionComponent : ICharacterComponent
    {
        ICharacterEntity MaxAggressionTarget { get; }


        void AddAggressionScore(ICharacterEntity aggressor, int aggrScore);
    }
}