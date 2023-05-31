public class InventoryItemData
{
    public ItemData itemData;
    public int elementsCount;


    public InventoryItemData(ItemData itemData = null, int elementsCount = 1)
    {
        this.itemData = itemData;
        this.elementsCount = elementsCount;
    }
}
