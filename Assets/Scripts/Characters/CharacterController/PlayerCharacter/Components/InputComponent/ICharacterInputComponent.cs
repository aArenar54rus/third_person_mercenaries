namespace Arenar.Character
{
    public interface ICharacterInputComponent : ICharacterComponent
    {
        void SetControlStatus(bool status);
    }
}