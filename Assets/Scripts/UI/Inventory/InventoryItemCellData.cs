using System;

namespace Arenar.Services.InventoryService
{
    [Serializable]
    public class InventoryItemCellData
    {
        public ItemInventoryData itemInventoryData;
        public int elementsCount;


        public bool IsLocked { get; private set; } = false;


        public bool StackIsFull
        {
            get
            {
                if (itemInventoryData == null)
                    return false;

                if (!itemInventoryData.CanStack)
                    return true;

                return elementsCount >= itemInventoryData.StackCountMax;
            }
        }
        
        
        public InventoryItemCellData(ItemInventoryData itemInventoryData = null, int elementsCount = 0)
        {
            this.itemInventoryData = itemInventoryData;
            this.elementsCount = elementsCount;
        }
        
        
        public void SetLock() =>
            IsLocked = true;

        public void SetUnlock()=>
            IsLocked = false;
    }
}