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
        private InventoryItemCellData equippedMeleeWeaponsCell;
        [JsonProperty]
        private InventoryItemCellData[] equippedFirearmWeaponsCells;
        [JsonProperty]
        private InventoryItemCellData[] inventoryBagCells;


        [JsonIgnore]
        public InventoryItemCellData EquippedMeleeWeaponsCell
        {
            get => equippedMeleeWeaponsCell;
            set => equippedMeleeWeaponsCell = value;
        }

        [JsonIgnore]
        public InventoryItemCellData[] EquippedFirearmWeaponCells
        {
            get => equippedFirearmWeaponsCells;
            set => equippedFirearmWeaponsCells = value;
        }
        
        [JsonIgnore]
        public InventoryItemCellData[] InventoryBagCells
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
        

        public void DataReInitialize(int inventoryBagCellsCount, int equippedWeaponCellsCount)
        {
            equippedMeleeWeaponsCell = new InventoryItemCellData(null, 0);
            
            if (equippedFirearmWeaponsCells == null)
            {
                equippedFirearmWeaponsCells = new InventoryItemCellData[equippedWeaponCellsCount];
                for (int i = 0; i < equippedFirearmWeaponsCells.Length; i++)
                    equippedFirearmWeaponsCells[i] = new InventoryItemCellData(null, 0);
            }
            
            if (inventoryBagCells == null)
            {
                inventoryBagCells = new InventoryItemCellData[inventoryBagCellsCount];
                for (int i = 0; i < inventoryBagCells.Length; i++)
                    inventoryBagCells[i] = new InventoryItemCellData(null, 0);
            }
        }
    }
}