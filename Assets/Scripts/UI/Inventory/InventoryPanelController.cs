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
            if (data.itemData != null)
                cell.SetItem(data);
            else
                cell.SetEmpty();

            _inventoryCells.Add(i, cell);
        }
    }
    
    
}
