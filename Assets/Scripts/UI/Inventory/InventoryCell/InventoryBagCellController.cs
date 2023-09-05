using Arenar.Services.InventoryService;
using TMPro;
using UnityEngine;


namespace Arenar.Services.UI
{
    public class InventoryBagCellController : InventoryCellController
    {
        [SerializeField] private TMP_Text _countText = default;
        
        
        public override void SetItem(InventoryItemData inventoryItemData)
        {
            base.SetItem(inventoryItemData);

            _countText.enabled = inventoryItemData.itemData.CanStack;
            _countText.text = $"{inventoryItemData.elementsCount}/{inventoryItemData.itemData.StackCountMax}";
        }
        
        public virtual void SetEmpty()
        {
            base.SetEmpty();
            
            _countText.enabled = false;
        }
    }
}