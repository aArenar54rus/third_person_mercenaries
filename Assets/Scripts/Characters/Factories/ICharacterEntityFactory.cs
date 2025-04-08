namespace Arenar.Character
{
    public interface ICharacterEntityFactory<TProduct>
    {
        TProduct Create(CharacterTypeKeys characterEntityType);
    }
}