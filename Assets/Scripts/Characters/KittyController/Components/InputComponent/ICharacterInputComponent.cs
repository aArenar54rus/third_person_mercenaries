using UnityEngine;


namespace Arenar.Character
{
    public interface ICharacterInputComponent : ICharacterComponent
    {
        PlayerInput PlayerInputs { get; }

        Vector2 MoveAction { get; }
        Vector2 LookAction { get; }

        bool JumpAction { get; }

        bool SprintAction { get; }
        
        bool InteractAction { get; }


        void SetControlStatus(bool status);
    }
}