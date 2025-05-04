using Newtonsoft.Json;

namespace Arenar.Services.InventoryService
{
    public class InventoryCellSaveData
    {
        [JsonProperty]
        private int itemId;
        [JsonProperty]
        public int itemCount;
        [JsonProperty]
        public int itemLevel;


        [JsonIgnore]
        public int ItemId => itemId;
        [JsonIgnore]
        public int ItemCount => itemCount;
        [JsonIgnore]
        public int ItemLevel => itemLevel;


        public void SetItem(InventoryCellData inventoryCellData)
        {
            itemId = (inventoryCellData != null) ? inventoryCellData.itemData.Id : -1;
            itemCount = inventoryCellData?.ElementsCount ?? 0;
            itemLevel = inventoryCellData?.itemLevel ?? 0;
        }
    }
}