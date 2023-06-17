namespace Arenar.Character
{
    public interface ICharacterRayCastComponent : ICharacterComponent
    {
        bool IsGroundedCheck();

        InteractableElement GetInteractableElementsOnCross();
    }
}