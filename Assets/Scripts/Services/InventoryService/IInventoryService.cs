using System;


namespace Arenar.Services.InventoryService
{
    public interface IInventoryService
    {
        event Action OnUpdateInventoryData;
        
        
        bool IsMassOverbalance { get; }

        int InventoryCellsCount { get; }
        
        float InventoryMass { get; }
        
        int InventoryMassMax { get; }


        bool TryAddItem(ItemData itemData,
            int count,
            out InventoryItemData restOfItems);
        
        bool TryAddItem(int itemIndex,
            int count,
            out InventoryItemData restOfItems);

        bool TryAddItemInCurrentCell(int cellIndex,
            ItemData itemData,
            int count,
            out InventoryItemData restOfItems);

        bool TryAddItemInCurrentCell(int cellIndex,
            int itemIndex,
            int count,
            out InventoryItemData restOfItems);

        void RemoveItemFromCell(int cellIndex,
            int count,
            out InventoryItemData restOfItems);

        bool IsEnoughItems(ItemData itemData, int neededCount);
        
        bool IsEnoughItems(int itemIndex, int neededCount);

        void RemoveItems(ItemData itemData,
            int neededCount,
            out InventoryItemData restOfItems);
        
        void RemoveItems(int itemIndex,
            int neededCount,
            out InventoryItemData restOfItems);
    }
}
