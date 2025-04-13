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
        [FormerlySerializedAs("_weaponInventoryData"),SerializeField] private FirearmWeaponInventoryItemData firearmWeaponInventoryData;
        

        public FirearmWeapon Weapon => _weapon;
        public FirearmWeaponInventoryItemData FirearmWeaponInventoryData => firearmWeaponInventoryData;
    }
}