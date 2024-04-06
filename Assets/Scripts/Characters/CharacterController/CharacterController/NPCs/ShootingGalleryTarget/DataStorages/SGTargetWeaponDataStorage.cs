using System;
using UnityEngine;


namespace Arenar.Character
{
    [Serializable]
    public class SGTargetWeaponDataStorage
    {
        [SerializeField] private FirearmWeapon _weapon;
        [SerializeField] private WeaponInventoryItemData _weaponInventoryData;
        

        public FirearmWeapon Weapon => _weapon;
        public WeaponInventoryItemData WeaponInventoryData => _weaponInventoryData;
    }
}