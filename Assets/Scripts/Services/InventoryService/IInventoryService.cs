using System;
using System.Collections.Generic;


namespace Arenar.Services.InventoryService
{
    public interface IInventoryService
    {
        event Action<List<int>> OnUpdateInventoryCells;
        event Action<ItemClothType> OnUpdateEquippedClothItemCell;
        event Action OnUpdateEquippedWeaponItem;
        
        
        bool IsMassOverbalance { get; }

        int InventoryCellsCount { get; }
        
        float InventoryMass { get; }
        
        int InventoryMassMax { get; }


        InventoryItemCellData GetInventoryItemData(int cellIndex);

        bool TryAddItems(ItemInventoryData itemInventoryData,
            int count,
            out InventoryItemCellData restOfItemsCell);
        
        bool TryAddItemInCurrentCell(int cellIndex,
            ItemInventoryData itemInventoryData,
            int count,
            out InventoryItemCellData restOfItemsCell);

        void RemoveItemFromCell(int cellIndex,
            int count,
            out InventoryItemCellData restOfItemsCell);

        bool IsEnoughItems(ItemInventoryData itemInventoryData, int neededCount);
        
        bool IsEnoughItems(int itemIndex, int neededCount);

        bool TryRemoveItems(ItemInventoryData itemInventoryData,
            int neededCount,
            out InventoryItemCellData restOfItemsCell);
        
        bool TryRemoveItems(int itemIndex,
            int neededCount,
            out InventoryItemCellData restOfItemsCell);

        InventoryItemCellData[] GetEquippedWeapons();

        InventoryItemCellData GetEquippedCloth(ItemClothType itemClothType);
    }
}
