using System;

namespace Arenar.Services.InventoryService
{
    [Serializable]
    public class InventoryCellData
    {
        public ItemData itemData;
        private int _itemLevel = 0;
        private int _elementsCount;
        
        
        public int ElementsCount
        {
            get => (itemData == null) ? 0 : _elementsCount;
            set => _elementsCount = value;
        }

        public int ItemLevel {
            get => _itemLevel;
            set
            {
                _itemLevel = value;
                if (_itemLevel < 1)
                    _itemLevel = 1;
            }
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
        
        public InventoryCellData(ItemData itemData = null, int elementsCount = 0)
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