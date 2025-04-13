using UnityEngine;


namespace Arenar.Items
{
    public interface IWeapon : IEquippedItem
    {
        public Vector3 RotationInHands { get; }
        float Damage { get; }
        WeaponType WeaponType { get; }
        float TimeBetweenAttacks { get; }
    }
}