using System;

namespace Arenar.Services.InventoryService
{
    [Serializable]
    public class InventoryItemCellData
    {
        public ItemData itemData;
        private int elementsCount;
        
        
        public int ElementsCount
        {
            get => (itemData == null) ? 0 : elementsCount;
            set => elementsCount = value;
        }
        
        public bool IsLocked { get; private set; } = false;
        
        public bool StackIsFull
        {
            get
            {
                if (itemData == null)
                    return false;

                if (!itemData.CanStack)
                    return true;

                return ElementsCount >= itemData.StackCountMax;
            }
        }
        
        public InventoryItemCellData(ItemData itemData = null, int elementsCount = 0)
        {
            this.itemData = itemData;
            this.ElementsCount = elementsCount;
        }
        
        public void SetLock() =>
            IsLocked = true;

        public void SetUnlock()=>
            IsLocked = false;
    }
}