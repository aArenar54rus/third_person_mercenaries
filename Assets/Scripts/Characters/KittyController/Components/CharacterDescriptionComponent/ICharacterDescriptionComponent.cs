namespace Arenar.Character
{
    public interface ICharacterDescriptionComponent : ICharacterComponent
    {
        string CharacterName { get; }
        
        string CharacterDescription { get; }
    }
}