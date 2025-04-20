using Arenar.Services.InventoryService;
using TMPro;
using UnityEngine;

namespace Arenar.Services.UI
{
    public class InventoryBagCellVisual : InventoryCellVisual
    {
        [SerializeField] 
        private TMP_Text itemNameText = default;
        [SerializeField] 
        private TMP_Text itemTypeText = default;
        [SerializeField] 
        private TMP_Text countText = default;
        
        
        public override void Initialize(int cellIndex, InventoryItemCellData inventoryItemCellData)
        {
            base.Initialize(cellIndex, inventoryItemCellData);

            if (inventoryItemCellData.itemData == null)
            {
                Debug.LogError("Emptiness cell data could not be initialized.");
                return;
            }

            itemNameText.text = inventoryItemCellData.itemData.NameKey;
            itemTypeText.text = inventoryItemCellData.itemData.ItemType.ToString();

            countText.enabled = inventoryItemCellData.itemData.CanStack;
            countText.text = $"{inventoryItemCellData.ElementsCount}/{inventoryItemCellData.itemData.StackCountMax}";
        }
        
        public virtual void SetEmpty()
        {
            base.SetEmpty();
            
            countText.enabled = false;
        }
    }
}