using UnityEngine;


namespace Arenar.Character
{
    public interface ICharacterRayCastComponent : ICharacterComponent
    {
        bool IsGrounded { get; }

        Vector3 RaycastPoint { get; }

        Transform ObjectOnCross { get; }
        
        InteractableElement InteractableElementsOnCross { get; }
        
        ICharacterEntity CharacterControllerOnCross { get; }
    }
}