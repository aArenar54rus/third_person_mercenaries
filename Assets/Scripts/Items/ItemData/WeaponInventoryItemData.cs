using UnityEngine;
using UnityEngine.Serialization;


namespace Arenar
{
    [CreateAssetMenu(menuName = "Items/WeaponItemInventoryData")]
    public class WeaponInventoryItemData : ItemInventoryData
    {
        [SerializeField] private WeaponType _weaponType;
        [FormerlySerializedAs("_firearmWeaponData"),SerializeField] private FirearmWeaponData firearmWeaponData;


        public WeaponType WeaponType => _weaponType;
        public FirearmWeaponData FirearmWeaponData => firearmWeaponData;
    }
}