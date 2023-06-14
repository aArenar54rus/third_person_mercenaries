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

        bool TryAddItemInCurrentCell(int cellIndex,
            ItemData itemData,
            int count,
            out InventoryItemData restOfItems);

        void RemoveItemFromCell(int cellIndex, out InventoryItemData restOfItems);
    }
}
