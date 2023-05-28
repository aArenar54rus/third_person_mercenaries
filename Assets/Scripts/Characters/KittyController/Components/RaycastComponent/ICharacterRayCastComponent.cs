namespace CatSimulator.Character
{
    public interface ICharacterRayCastComponent : ICharacterComponent
    {
        bool IsGroundedCheck();
    }
}