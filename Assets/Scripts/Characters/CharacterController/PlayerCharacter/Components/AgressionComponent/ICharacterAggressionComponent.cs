using System.Collections.Generic;


namespace Arenar.Character
{
    public interface ICharacterAggressionComponent : ICharacterComponent
    {
        Dictionary<ICharacterEntity, int> CharacterAggressionScores { get; }
        ICharacterEntity AggressionTarget { get; }


        void AddAggressionScore(ICharacterEntity aggressor, int aggrScore);
    }
}