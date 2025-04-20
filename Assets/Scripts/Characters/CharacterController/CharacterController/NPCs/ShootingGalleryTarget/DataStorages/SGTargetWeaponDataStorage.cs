using Arenar.Items;
using System;
using UnityEngine;
using UnityEngine.Serialization;


namespace Arenar.Character
{
    [Serializable]
    public class SGTargetWeaponDataStorage
    {
        [SerializeField] private FirearmWeapon _weapon;
        [FormerlySerializedAs("firearmWeaponInventoryData"),FormerlySerializedAs("_weaponInventoryData"),SerializeField] private FirearmWeaponItemData firearmWeaponData;
        

        public FirearmWeapon Weapon => _weapon;
        public FirearmWeaponItemData FirearmWeaponData => firearmWeaponData;
    }
}