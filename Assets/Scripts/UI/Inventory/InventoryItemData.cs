namespace Arenar.Services.InventoryService
{
    public class InventoryItemData
    {
        public ItemData itemData;
        public int elementsCount;


        public bool StackIsFull
        {
            get
            {
                if (itemData == null)
                    return false;

                if (!itemData.CanStack)
                    return true;

                return elementsCount >= itemData.StackCountMax;
            }
        }


        public InventoryItemData(ItemData itemData = null, int elementsCount = 1)
        {
            this.itemData = itemData;
            this.elementsCount = elementsCount;
        }
    }
}