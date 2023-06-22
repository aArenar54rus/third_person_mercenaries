using System.Collections.Generic;
using Arenar.Services.InventoryService;
using UnityEngine;
using Zenject;


namespace Arenar
{
    public class InventoryPanelController : MonoBehaviour
    {
        [SerializeField] private InventoryCellController _cellPrefab = default;
        [SerializeField] private RectTransform _cellsContainer = default;

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
                cell.Initialize(i);
                UpdateCell(cell);
                _inventoryCells.Add(i, cell);
            }

            inventoryService.OnUpdateInventoryCells += OnUpdateCells;
        }

        private void OnUpdateCells(List<int> cellIndexes)
        {
            foreach (var cellIndex in cellIndexes)
                UpdateCell(_inventoryCells[cellIndex]);
        }

        private void UpdateCell(InventoryCellController cell)
        {
            InventoryItemData data = _inventoryService.GetInventoryItemData(cell.CellIndex);
                
            if (data.itemData != null)
                cell.SetItem(data);
            else
                cell.SetEmpty();
        }
    }
}