using System;
using System.Collections.Generic;
using Arenar.Services.InventoryService;
using Newtonsoft.Json;


namespace Arenar.Services.SaveAndLoad
{
    [Serializable]
    public class InventorySaveDelegate
    {
        [JsonProperty] public Dictionary<int, InventoryCellData> equippedWeapons;
    }
}