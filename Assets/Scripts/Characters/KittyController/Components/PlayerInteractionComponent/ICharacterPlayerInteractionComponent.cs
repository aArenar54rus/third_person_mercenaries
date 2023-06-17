namespace Arenar.Character
{
    public interface ICharacterPlayerInteractionComponent : ICharacterComponent
    {
        bool CanInteract { get; }


        void InteractWithInteractObject(InteractableElement element);
    }
}