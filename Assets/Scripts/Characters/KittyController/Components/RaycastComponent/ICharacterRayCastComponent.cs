namespace Arenar.Character
{
    public interface ICharacterRayCastComponent : ICharacterComponent
    {
        bool IsGroundedCheck();

        bool TryGetObjectOnCross(out UnityEngine.Transform objectTransform);

        bool TryGetObjectOnCross(out UnityEngine.Transform objectTransform, out UnityEngine.Vector3 raycastPoint);

        InteractableElement GetInteractableElementsOnCross();
    }
}