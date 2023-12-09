using Arenar.Services.InventoryService;
using TMPro;
using UnityEngine;


namespace Arenar.Services.UI
{
    public class InventoryBagCellController : InventoryCellController
    {
        [SerializeField] private TMP_Text _countText = default;
        
        
        public override void SetItem(InventoryItemCellData inventoryItemCellData)
        {
            base.SetItem(inventoryItemCellData);

            _countText.enabled = inventoryItemCellData.itemInventoryData.CanStack;
            _countText.text = $"{inventoryItemCellData.elementsCount}/{inventoryItemCellData.itemInventoryData.StackCountMax}";
        }
        
        public virtual void SetEmpty()
        {
            base.SetEmpty();
            
            _countText.enabled = false;
        }
    }
}