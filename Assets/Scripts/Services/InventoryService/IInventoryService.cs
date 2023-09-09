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


        InventoryItemData GetInventoryItemData(int cellIndex);

        bool TryAddItems(ItemData itemData,
            int count,
            out InventoryItemData restOfItems);
        
        bool TryAddItemInCurrentCell(int cellIndex,
            ItemData itemData,
            int count,
            out InventoryItemData restOfItems);

        void RemoveItemFromCell(int cellIndex,
            int count,
            out InventoryItemData restOfItems);

        bool IsEnoughItems(ItemData itemData, int neededCount);
        
        bool IsEnoughItems(int itemIndex, int neededCount);

        bool TryRemoveItems(ItemData itemData,
            int neededCount,
            out InventoryItemData restOfItems);
        
        bool TryRemoveItems(int itemIndex,
            int neededCount,
            out InventoryItemData restOfItems);

        InventoryItemData[] GetEquippedWeapons();

        InventoryItemData GetEquippedCloth(ItemClothType itemClothType);
    }
}
