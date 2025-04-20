using UnityEngine;

namespace Arenar.Items
{
    [CreateAssetMenu(menuName = "Items/MeleeWeaponInventoryItemData")]
    public class MeleeWeaponItemData : ItemData
    {
        [SerializeField]
        private MeleeWeaponData meleeWeaponData;
        
        
        public WeaponType WeaponType => WeaponType.Melee;
        public MeleeWeaponData MeleeWeaponData => meleeWeaponData;
    }
}