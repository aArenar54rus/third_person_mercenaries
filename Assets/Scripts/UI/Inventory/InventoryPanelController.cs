using System.Collections.Generic;
using Arenar.Services.InventoryService;
using TMPro;
using UnityEngine;
using Zenject;


public class InventoryPanelController : MonoBehaviour
{
    [SerializeField] private InventoryCellController _cellPrefab = default;
    [SerializeField] private RectTransform _cellsContainer = default;
    [SerializeField] private TMP_Text _moneyCountText = default;

    private Dictionary<int, InventoryCellController> _inventoryCells;
    private IInventoryService _inventoryService;

    
    [Inject]
    public void Construct(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
        
        _inventoryCells = new Dictionary<int, InventoryCellController>(inventoryService.InventoryCellsCount);
        for (int i = 0; i < inventoryService.InventoryCellsCount; i++)
        {
            InventoryCellController cell = GameObject.Instantiate(_cellPrefab, _cellsContainer);
           
            InventoryItemData data = _inventoryService.GetInventoryItemData(i);
            cell.AddItem(data);
            
            _inventoryCells.Add(i, cell);
        }
    }
    
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
