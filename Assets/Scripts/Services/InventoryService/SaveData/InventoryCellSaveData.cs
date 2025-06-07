using Newtonsoft.Json;

namespace Arenar.Services.InventoryService
{
    public class InventoryCellSaveData
    {
        [JsonProperty]
        public int itemId;
        [JsonProperty]
        public int itemCount;
        [JsonProperty]
        public int itemLevel;


        public void UpdateData(InventoryCellData data) {
            bool isEmpty = (data == null || data.itemData == null);
            itemId = !isEmpty ? data.itemData.Id : -1;
            itemCount = isEmpty ? data.ElementsCount : 0;
            itemLevel = isEmpty ? data.ItemLevel : 0;
        }
    }
}