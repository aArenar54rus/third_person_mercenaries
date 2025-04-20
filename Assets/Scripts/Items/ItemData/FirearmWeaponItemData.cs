using UnityEngine;
using UnityEngine.Serialization;


namespace Arenar.Items
{
    [CreateAssetMenu(menuName = "Items/FirearmWeaponInventoryItemData")]
    public class FirearmWeaponItemData : ItemData
    {
        [FormerlySerializedAs("_firearmWeaponData"),SerializeField]
        private FirearmWeaponData firearmWeaponData;


        public WeaponType WeaponType => WeaponType.Firearm;
        public FirearmWeaponData FirearmWeaponData => firearmWeaponData;
    }
}