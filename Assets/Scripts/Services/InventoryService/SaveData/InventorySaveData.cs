using System;
using Arenar.Services.InventoryService;
using Newtonsoft.Json;
using System.Collections.Generic;


namespace Arenar
{
    [Serializable]
    public class InventorySaveData
    {
        [JsonProperty]
        private bool isAddedContentEarly = false;
        [JsonProperty]
        private InventoryCellSaveData equippedMeleeWeaponsCell;
        [JsonProperty]
        private InventoryCellSaveData[] equippedFirearmWeaponsCells;
        [JsonProperty]
        private InventoryCellSaveData[] inventoryBagCells;


        [JsonIgnore]
        public InventoryCellSaveData EquippedMeleeWeaponsCell
        {
            get => equippedMeleeWeaponsCell;
            set => equippedMeleeWeaponsCell = value;
        }

        [JsonIgnore]
        public InventoryCellSaveData[] EquippedFirearmWeaponCells
        {
            get => equippedFirearmWeaponsCells;
            set => equippedFirearmWeaponsCells = value;
        }
        
        [JsonIgnore]
        public InventoryCellSaveData[] InventoryBagCells
        {
            get => inventoryBagCells;
            set => inventoryBagCells = value;
        }
        
        [JsonIgnore]
        public bool IsAddedContentEarly
        {
            get => isAddedContentEarly;
            set => isAddedContentEarly = value;
        }
    }
}