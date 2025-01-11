using UnityEngine;


namespace Arenar
{
    public interface IFirearmWeaponAttackItemComponent : IEquippedItemComponent
    {
        void MakeShoot(Transform muzzle, Vector3 direction, DamageData damageData);
    }
}