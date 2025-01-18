using UnityEngine;

namespace Arenar
{
    public interface IEquippedItemAimComponent : IEquippedItemComponent
    {
        void SetAimStatus(bool isAiming);
        
        void OnUpdate(Vector3 position);
    }
}