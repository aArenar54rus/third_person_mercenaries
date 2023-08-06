using Arenar.Services.UI;
using UnityEngine;


namespace Arenar.UI
{
    public class InventoryEquipCanvasLayer : CanvasWindowLayer
    {
        [SerializeField] private SerializableDictionary<ItemClothType, InventoryEquipCellController> clothItemCells;
        [SerializeField] private InventoryEquipCellController weaponCell;
        

        public SerializableDictionary<ItemClothType, InventoryEquipCellController> ClothItemCells => clothItemCells;
        public InventoryEquipCellController WeaponCell => weaponCell;
    }
}