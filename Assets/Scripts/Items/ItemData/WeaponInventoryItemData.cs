using UnityEngine;


namespace Arenar
{
    [CreateAssetMenu(menuName = "Items/WeaponItemInventoryData")]
    public class WeaponInventoryItemData : ItemInventoryData
    {
        [SerializeField] private WeaponType _weaponType;


        public WeaponType WeaponType => _weaponType;
    }
}