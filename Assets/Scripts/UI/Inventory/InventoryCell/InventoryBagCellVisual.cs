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
        
        
        public override void Initialize(int cellIndex, InventoryCellData inventoryCellData)
        {
            base.Initialize(cellIndex, inventoryCellData);

            if (inventoryCellData.itemData == null)
            {
                Debug.LogError("Emptiness cell data could not be initialized.");
                return;
            }

            itemNameText.text = inventoryCellData.itemData.NameKey;
            itemTypeText.text = inventoryCellData.itemData.ItemType.ToString();

            countText.enabled = inventoryCellData.itemData.CanStack;
            countText.text = $"{inventoryCellData.ElementsCount}/{inventoryCellData.itemData.StackCountMax}";
        }
        
        public virtual void SetEmpty()
        {
            base.SetEmpty();
            
            countText.enabled = false;
        }
    }
}