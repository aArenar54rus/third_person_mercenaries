using UnityEngine;

namespace Arenar.Character
{
    public interface ICharacterMovementComponent : ICharacterComponent
    {
        MovementContainer MovementContainer { get; set; }
        
        
        void Move(Vector3 direction);

        void Rotation(Vector3 direction);
    }
}