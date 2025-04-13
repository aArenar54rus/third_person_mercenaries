using UnityEngine;

namespace Arenar.Character
{
    public interface ICharacterMovementComponent : ICharacterComponent
    {
        MovementContainer MovementContainer { get; set; }
        
        
        void Move(Vector2 direction, bool isSprint);
        
        void JumpAndGravity(bool isJumpAction);

        void Rotation(Vector2 direction);
    }
}