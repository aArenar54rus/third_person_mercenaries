namespace CatSimulator.Character
{
    public interface ICharacterMovementComponent : ICharacterComponent
    {
        void Move();

        void Rotation();
    }
}