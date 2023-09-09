using UnityEngine;


namespace Arenar.Services.UI
{
    public class InventoryEquipCanvasLayer : CanvasWindowLayer
    {
        [SerializeField] private SerializableDictionary<ItemClothType, InventoryEquipCellController> clothItemCells;
        [SerializeField] private InventoryEquipCellController[] weaponCells;
        

        public SerializableDictionary<ItemClothType, InventoryEquipCellController> ClothItemCells => clothItemCells;
        public InventoryEquipCellController[] WeaponCells => weaponCells;
    }
}