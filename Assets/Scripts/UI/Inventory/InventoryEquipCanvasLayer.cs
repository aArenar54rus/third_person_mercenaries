using UnityEngine;


namespace Arenar.Services.UI
{
    public class InventoryEquipCanvasLayer : CanvasWindowLayer
    {
        [SerializeField]
        private InventoryEquipCellVisual meleeWeaponCell;
        [SerializeField]
        private SerializableDictionary<ItemClothType, InventoryEquipCellVisual> clothItemCells;
        [SerializeField]
        private InventoryEquipCellVisual[] weaponCells;
        

        public InventoryEquipCellVisual MeleeWeaponCell => meleeWeaponCell;
        public SerializableDictionary<ItemClothType, InventoryEquipCellVisual> ClothItemCells => clothItemCells;
        public InventoryEquipCellVisual[] WeaponCells => weaponCells;
    }
}