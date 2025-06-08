using System;
using Arenar.Services.InventoryService;
using Newtonsoft.Json;
using System.Collections.Generic;


namespace Arenar
{
    [Serializable]
    public struct InventorySaveData
    {
        [JsonProperty]
        private bool isAddedContentEarly;
        [JsonProperty]
        private InventoryCellSaveData equippedMeleeWeaponsCell;
        [JsonProperty]
        private InventoryCellSaveData[] firearmWeaponsCells;
        [JsonProperty]
        private InventoryCellSaveData[] inventoryBagCells;

        
        [JsonIgnore]
        public InventoryCellSaveData EquippedMeleeWeaponsCell
        {
            get => equippedMeleeWeaponsCell;
            set => equippedMeleeWeaponsCell = value;
        }

        [JsonIgnore]
        public InventoryCellSaveData[] FirearmWeapons
        {
            get => firearmWeaponsCells;
            set => firearmWeaponsCells = value;
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