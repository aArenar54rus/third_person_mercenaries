namespace Arenar.Character
{
    public interface ICharacterDataStorage<TCharacterData>
    {
        TCharacterData Data { get; }
    }
}