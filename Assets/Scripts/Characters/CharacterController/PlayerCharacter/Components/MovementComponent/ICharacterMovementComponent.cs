using UnityEngine;

namespace Arenar.Character
{
    public interface ICharacterMovementComponent : ICharacterComponent
    {
        void Move(Vector3 direction);

        void Rotation(Vector3 direction);
    }
}