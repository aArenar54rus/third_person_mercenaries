namespace Arenar.Character
{
    public interface ICharacterAimComponent : ICharacterComponent
    {
       bool IsAim { get; }
       
       float AimProgress { get; }
    }
}