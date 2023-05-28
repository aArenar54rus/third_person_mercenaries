namespace CatSimulator.Character
{
    public interface ICharacterDataStorage<TCharacterData>
    {
        TCharacterData Data { get; }
    }
}