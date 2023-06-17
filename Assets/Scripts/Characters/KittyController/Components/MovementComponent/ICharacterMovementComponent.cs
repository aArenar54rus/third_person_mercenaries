namespace Arenar.Character
{
    public interface ICharacterMovementComponent : ICharacterComponent
    {
        void Move();

        void Rotation();
    }
}