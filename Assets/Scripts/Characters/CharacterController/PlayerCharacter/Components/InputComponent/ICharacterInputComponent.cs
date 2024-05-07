using UnityEngine;


namespace Arenar.Character
{
    public interface ICharacterInputComponent : ICharacterComponent
    {
        Vector2 MoveAction { get; }
        Vector2 LookAction { get; }

        bool JumpAction { get; }

        bool SprintAction { get; }
        
        bool InteractAction { get; }
        
        bool AimAction { get; }
        
        bool ReloadAction { get; }
        
        bool AttackAction { get; }
        
        bool AimContinueAction { get; }
        

        void SetControlStatus(bool status);
    }
}