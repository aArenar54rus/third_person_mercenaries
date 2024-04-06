using System.ComponentModel;


namespace Arenar.Character
{
    public interface ISearchTargetComponent : ICharacterComponent
    {
        public ICharacterEntity CharacterEntityTarget { get; }


        void AddAggression(int aggression, ICharacterEntity aggressionCharacter);
    }
}