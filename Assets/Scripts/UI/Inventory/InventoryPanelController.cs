using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryPanelController : MonoBehaviour
{
    [SerializeField] private InventoryCellController[] inventoryCells = default;
    [SerializeField] private TMP_Text moneyCountText = default;
    
    
    public void AddToInventory(InventoryItemData inventoryItemData)
    {
        if (inventoryItemData.itemData is MoneyItemData)
        {
            UpdateMoneyCounterVisual();
            return;
        }
        
        if (inventoryItemData.itemData.CanStack)
        {
            while (inventoryItemData != null)
            {
                bool isFindCellWithSameItem = false;
                
                foreach (var cell in inventoryCells)
                {
                    if (cell.CurrentInventoryItemData == null)
                        continue;

                    if (!cell.IsSameItem(inventoryItemData.itemData)
                        || !cell.CanAddToCurrentItem)
                        continue;
                    
                    cell.TryAddCountToCurrentItem(inventoryItemData, out inventoryItemData);
                    isFindCellWithSameItem = true;
                    break;
                }
                
                if (isFindCellWithSameItem)
                    continue;
            }
        }

        if (inventoryItemData == null)
            return;

        AddItem(inventoryItemData);


        void AddItem(InventoryItemData inventoryItemData)
        {
            foreach (var cell in inventoryCells)
            {
                if (cell.CurrentInventoryItemData != null)
                    continue;

                cell.AddItem(inventoryItemData);
                return;
            }
        }
    }

    private InventoryCellController GetCell(ItemData addedItemData)
    {
        InventoryCellController neededCellController = null;

        // if can stack - first find cell with sameItem with free places for new items
        if (addedItemData.CanStack)
        {
            foreach (var cell in inventoryCells)
            {
                if (cell.CurrentInventoryItemData == null)
                    continue;

                if (cell.IsSameItem(addedItemData)
                    && cell.CanAddToCurrentItem)
                    return cell;
            }
        }
        
        foreach (var cell in inventoryCells)
        {
            if (cell.CurrentInventoryItemData == null)
            {
                
            }
            else
            {
                
            }
        }
    }

    private void UpdateMoneyCounterVisual()
    {
        
    }
}
