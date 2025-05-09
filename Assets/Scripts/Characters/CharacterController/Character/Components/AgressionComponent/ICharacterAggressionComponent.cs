namespace Arenar.Character
{
    public partial interface ICharacterAggressionComponent : ICharacterComponent
    {
        ICharacterEntity MaxAggressionTarget { get; }


        void AddAggressionScore(ICharacterEntity aggressor, int aggrScore);
    }
}