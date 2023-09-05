using System.Collections;
using System.Collections.Generic;
using Arenar.Services.InventoryService;
using UnityEngine;


namespace Arenar.Services.SaveAndLoad
{
    public class InventorySaveDelegate : SaveDelegate
    {
        public Dictionary<int, InventoryItemData> equippedWeapons;
    }
}