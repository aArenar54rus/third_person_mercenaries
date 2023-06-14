using TMPro;
using UnityEngine;


public class InventoryPanelController : MonoBehaviour
{
    [SerializeField] private InventoryCellController[] _inventoryCells = default;
    [SerializeField] private TMP_Text _moneyCountText = default;
    
    
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
                
                foreach (var cell in _inventoryCells)
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
            foreach (var cell in _inventoryCells)
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
        // if can stack - first find cell with same item with free places for new items
        if (addedItemData.CanStack)
        {
            foreach (var cell in _inventoryCells)
            {
                if (cell.CurrentInventoryItemData == null)
                    continue;

                if (cell.IsSameItem(addedItemData)
                    && cell.CanAddToCurrentItem)
                    return cell;
            }
        }
        
        // search free cell
        foreach (var cell in _inventoryCells)
        {
            if (cell.CurrentInventoryItemData == null)
                return cell;
        }

        return null;
    }

    private void UpdateMoneyCounterVisual()
    {
        
    }
}
