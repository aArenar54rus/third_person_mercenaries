using System;
using Arenar.Services.InventoryService;
using Newtonsoft.Json;


namespace Arenar
{
    [Serializable]
    public class InventorySaveData
    {
        [JsonProperty] private bool _isAddedContentEarly = false;
        [JsonProperty] private InventoryItemCellData[] _equippedWeaponsCells;
        [JsonProperty] private InventoryItemCellData[] _inventoryCells;


        [JsonIgnore]
        public InventoryItemCellData[] EquippedWeaponCells
        {
            get => _equippedWeaponsCells;
            set => _equippedWeaponsCells = value;
        }
        
        [JsonIgnore]
        public InventoryItemCellData[] InventoryCells
        {
            get => _inventoryCells;
            set => _inventoryCells = value;
        }
        
        [JsonIgnore]
        public bool IsAddedContentEarly
        {
            get => _isAddedContentEarly;
            set => _isAddedContentEarly = value;
        }
        

        public void DataReInitialize(int inventoryCellsCount, int equippedWeaponCellsCount)
        {
            if (_equippedWeaponsCells == null)
            {
                _equippedWeaponsCells = new InventoryItemCellData[equippedWeaponCellsCount];
                for (int i = 0; i < _equippedWeaponsCells.Length; i++)
                    _equippedWeaponsCells[i] = new InventoryItemCellData(null, 0);
            }
            
            if (_inventoryCells == null)
            {
                _inventoryCells = new InventoryItemCellData[inventoryCellsCount];
                for (int i = 0; i < _inventoryCells.Length; i++)
                    _inventoryCells[i] = new InventoryItemCellData(null, 0);
            }
        }
    }
}