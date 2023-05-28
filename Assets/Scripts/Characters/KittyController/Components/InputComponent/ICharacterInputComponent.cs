using UnityEngine;


namespace CatSimulator.Character
{
    public interface ICharacterInputComponent : ICharacterComponent
    {
        PlayerInput PlayerInputs { get; }

        Vector2 MoveAction { get; }
        Vector2 LookAction { get; }

        bool JumpAction { get; }

        bool SprintAction { get; }


        void SetControlStatus(bool status);
    }
}