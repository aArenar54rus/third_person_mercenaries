using System;
using System.Collections.Generic;
using TakeTop.PreferenceSystem;
using UnityEngine;


namespace Arenar.Services.InventoryService
{
    public class InventoryService : IInventoryService
    {
        public event Action<List<int>> OnUpdateInventoryCells;
        public event Action<ItemClothType> OnUpdateEquippedClothItemCell;
        public event Action OnUpdateEquippedWeaponItem;
        
        
        private Dictionary<ItemClothType, InventoryItemCellData> _equippedClothItems;

        private float _currentInventoryMass = 0.0f;
        private InventoryOptionsSOData.Parameters _parameters;

        private IPreferenceManager _preferenceManager;
        private InventorySaveData _inventorySaveData;


        public bool IsMassOverbalance =>
            _currentInventoryMass > InventoryMassMax;

        public int InventoryCellsCount =>
            _parameters.DefaultInventoryCellsCount;

        public float InventoryMass =>
            _currentInventoryMass;

        public int InventoryMassMax =>
            _parameters.DefaultMassMax;

        public Dictionary<ItemClothType, InventoryItemCellData> EquippedClothItems =>
            _equippedClothItems;


        public InventoryService(InventoryOptionsSOData.Parameters parameters, IPreferenceManager preferenceManager)
        {
            _parameters = parameters;
            _preferenceManager = preferenceManager;
            Initialize();
        }

        public InventoryItemCellData GetInventoryItemData(int cellIndex) =>
            _inventorySaveData.InventoryCells[cellIndex];

        public InventoryItemCellData[] GetEquippedWeapons() =>
            _inventorySaveData.EquippedWeaponCells;

        public InventoryItemCellData GetEquippedCloth(ItemClothType itemClothType)
        {
            if (_equippedClothItems.ContainsKey(itemClothType))
            {
                Debug.LogError($"Unknown cloth type {itemClothType}!");
                return null;
            }
            
            return _equippedClothItems[itemClothType];
        }

        public bool TryAddItems(ItemInventoryData itemInventoryData, int count, out InventoryItemCellData restOfItemsCell)
        {
            // first, check mass
            if (itemInventoryData.ItemMass * count > InventoryMassMax - InventoryMass)
            {
                restOfItemsCell = new InventoryItemCellData(itemInventoryData, count);
                return false;
            }

            if (itemInventoryData.ItemType == ItemType.Weapon)
            {
                for (int i = 0; i < _inventorySaveData.EquippedWeaponCells.Length; i++)
                {
                    _inventorySaveData.EquippedWeaponCells[i] ??= new InventoryItemCellData();
                    if (_inventorySaveData.EquippedWeaponCells[i].itemInventoryData != null)
                        continue;
                    
                    _inventorySaveData.EquippedWeaponCells[i].itemInventoryData = itemInventoryData;
                    _inventorySaveData.EquippedWeaponCells[i].elementsCount = 1;
                    restOfItemsCell = null;
                    CalculateMass();
                    OnUpdateEquippedWeaponItem?.Invoke();
                    return true;
                }
            }

            if (!itemInventoryData.CanStack)
                return TryAddInFreeCell(itemInventoryData, count, out restOfItemsCell);

            //check cells with same item
            List<int> updatedCellIndexes = new List<int>();
            for (int i = 0; i < _inventorySaveData.InventoryCells.Length; i++)
            {
                if (_inventorySaveData.InventoryCells[i].itemInventoryData == null)
                    continue;
                
                if (_inventorySaveData.InventoryCells[i].itemInventoryData.Id != itemInventoryData.Id
                    || _inventorySaveData.InventoryCells[i].StackIsFull)
                    continue;

                _inventorySaveData.InventoryCells[i].elementsCount += count;
                count = _inventorySaveData.InventoryCells[i].elementsCount - _inventorySaveData.InventoryCells[i].itemInventoryData.StackCountMax;
                updatedCellIndexes.Add(i);
                
                if (count > 0)
                    continue;
                
                CalculateMass();
                OnUpdateInventoryCells?.Invoke(new List<int>(updatedCellIndexes));
                
                restOfItemsCell = null;
                return true;
            }
            
            SaveData();
            
            return TryAddInFreeCell(itemInventoryData, count, out restOfItemsCell);
        }

        public bool TryAddItemInCurrentCell(int cellIndex,
                                            ItemInventoryData itemInventoryData,
                                            int count,
                                            out InventoryItemCellData restOfItemsCell)
        {
            InventoryItemCellData cellDataCell = GetInventoryItemData(cellIndex); 

            if (!itemInventoryData.CanStack)
            {
                if (cellDataCell.itemInventoryData == null)
                {
                    cellDataCell.itemInventoryData = itemInventoryData;
                    cellDataCell.elementsCount = count;
                    CalculateMass();
                    restOfItemsCell = null;
                    SaveData();
                    
                    OnUpdateInventoryCells?.Invoke(new List<int>(cellIndex));
                    return true;
                }

                restOfItemsCell = new InventoryItemCellData(itemInventoryData, count);
                return false;
            }

            if (cellDataCell.itemInventoryData.Id != itemInventoryData.Id
                || cellDataCell.StackIsFull)
            {
                restOfItemsCell = new InventoryItemCellData(itemInventoryData, count);
                return false;
            }

            cellDataCell.elementsCount += count;
            count = cellDataCell.elementsCount - cellDataCell.itemInventoryData.StackCountMax;
            
            CalculateMass();
            SaveData();
            OnUpdateInventoryCells?.Invoke(new List<int>(1){ cellIndex });

            if (count > 0)
            {
                restOfItemsCell = new InventoryItemCellData(itemInventoryData, count);
                return false;
            }
            
            restOfItemsCell = null;
            return true;
        }

        public void RemoveItemFromCell(int cellIndex, int count, out InventoryItemCellData restOfItemsCell)
        {
            var dataCell = GetInventoryItemData(cellIndex);
            if (dataCell.elementsCount < count)
            {
                restOfItemsCell = null;
                return;
            }
            
            restOfItemsCell = new InventoryItemCellData(dataCell.itemInventoryData, count);
            dataCell.elementsCount -= count;
            if (dataCell.elementsCount == 0)
                dataCell.itemInventoryData = null;
            
            CalculateMass();
            SaveData();
            OnUpdateInventoryCells?.Invoke(new List<int>(cellIndex));
        }

        public bool IsEnoughItems(ItemInventoryData itemInventoryData, int neededCount)
        {
            return IsEnoughItems(itemInventoryData.Id, neededCount);
        }

        public bool IsEnoughItems(int itemIndex, int neededCount)
        {
            int counter = 0;
            
            foreach (var inventoryCell in _inventorySaveData.InventoryCells)
            {
                if (inventoryCell.itemInventoryData.Id != itemIndex)
                    continue;

                counter += inventoryCell.elementsCount;
                if (counter >= neededCount)
                    return true;
            }

            return false;
        }

        public bool TryRemoveItems(ItemInventoryData itemInventoryData, int neededCount, out InventoryItemCellData restOfItemsCell)
        {
            if (!IsEnoughItems(itemInventoryData.Id, neededCount))
            {
                restOfItemsCell = null;
                return false;
            }

            int counter = 0;
            List<int> changedCellsIndexes = new List<int>();
            
            for (int i = 0; i < _inventorySaveData.InventoryCells.Length; i++)
            {
                if (_inventorySaveData.InventoryCells[i].itemInventoryData.Id != itemInventoryData.Id)
                    continue;

                changedCellsIndexes.Add(i);
                if (_inventorySaveData.InventoryCells[i].elementsCount <= neededCount)
                {
                    counter += _inventorySaveData.InventoryCells[i].elementsCount;
                    neededCount -= _inventorySaveData.InventoryCells[i].elementsCount;
                    _inventorySaveData.InventoryCells[i].itemInventoryData = null;
                    _inventorySaveData.InventoryCells[i].elementsCount = 0;

                    if (neededCount == 0)
                        break;
                }
                else
                {
                    counter += neededCount;
                    _inventorySaveData.InventoryCells[i].elementsCount -= neededCount;
                    break;
                }
            }

            CalculateMass();
            SaveData();
            OnUpdateInventoryCells?.Invoke(changedCellsIndexes);
            restOfItemsCell = new InventoryItemCellData(itemInventoryData, counter);
            return true;
        }

        public bool TryRemoveItems(int itemIndex, int neededCount, out InventoryItemCellData restOfItemsCell)
        {
            if (!IsEnoughItems(itemIndex, neededCount))
            {
                restOfItemsCell = null;
                return false;
            }

            int counter = 0;

            ItemInventoryData neededItemInventoryData = null;
            List<int> changedCellsIndexes = new List<int>();
            for (int i = 0; i < _inventorySaveData.InventoryCells.Length; i++)
            {
                if (_inventorySaveData.InventoryCells[i].itemInventoryData.Id != itemIndex)
                    continue;
                
                changedCellsIndexes.Add(i);
                neededItemInventoryData = _inventorySaveData.InventoryCells[i].itemInventoryData;
                if (_inventorySaveData.InventoryCells[i].elementsCount <= neededCount)
                {
                    counter += _inventorySaveData.InventoryCells[i].elementsCount;
                    neededCount -= _inventorySaveData.InventoryCells[i].elementsCount;
                    _inventorySaveData.InventoryCells[i].itemInventoryData = null;
                    _inventorySaveData.InventoryCells[i].elementsCount = 0;

                    if (neededCount == 0)
                        break;
                }
                else
                {
                    counter += neededCount;
                    _inventorySaveData.InventoryCells[i].elementsCount -= neededCount;
                    break;
                }
            }

            CalculateMass();
            SaveData();
            OnUpdateInventoryCells?.Invoke(changedCellsIndexes);
            restOfItemsCell = new InventoryItemCellData(neededItemInventoryData, counter);
            return true;
        }

        private void Initialize()
        {
            LoadData();
            
            /*_inventoryItemDataCells = new Dictionary<int, InventoryItemCellData>(InventoryCellsCount);
            for (int i = 0; i < InventoryCellsCount; i++)
                _inventoryItemDataCells.Add(i, new InventoryItemCellData(null, 0));

            _equippedClothItems = new Dictionary<ItemClothType, InventoryItemCellData>();
            _equippedClothItems.Add(ItemClothType.Head, new InventoryItemCellData(null, 0));
            _equippedClothItems.Add(ItemClothType.Body, new InventoryItemCellData(null, 0));
            _equippedClothItems.Add(ItemClothType.Hands, new InventoryItemCellData(null, 0));
            _equippedClothItems.Add(ItemClothType.Foots, new InventoryItemCellData(null, 0));*/
            
            CalculateMass();
        }

        private bool TryAddInFreeCell(ItemInventoryData itemInventoryData, int count, out InventoryItemCellData restOfItemsCell)
        {
            for(int i = 0; i < _inventorySaveData.InventoryCells.Length; i++)
            {
                if (_inventorySaveData.InventoryCells[i].itemInventoryData != null)
                    continue;
                    
                _inventorySaveData.InventoryCells[i].itemInventoryData = itemInventoryData;
                _inventorySaveData.InventoryCells[i].elementsCount = count;
                CalculateMass();
                SaveData();
                restOfItemsCell = null;
                    
                OnUpdateInventoryCells?.Invoke(new List<int>(1){ i });

                return true;
            }

            restOfItemsCell = new InventoryItemCellData(itemInventoryData, count);
            return false;
        }

        private void CalculateMass()
        {
            _currentInventoryMass = 0.0f;
            foreach (var inventoryItemData in _inventorySaveData.InventoryCells)
            {
                if (inventoryItemData == null
                    || inventoryItemData.itemInventoryData == null)
                    continue;

                _currentInventoryMass +=
                    inventoryItemData.elementsCount * inventoryItemData.itemInventoryData.ItemMass;
            }
            
            /*
            foreach (var equippedClothItem in _equippedClothItems)
            {
                if (equippedClothItem.Value == null
                    || equippedClothItem.Value.itemInventoryData == null)
                    continue;

                _currentInventoryMass += equippedClothItem.Value.itemInventoryData.ItemMass;
            }
            */
            
            foreach (var equippedWeapon in _inventorySaveData.EquippedWeaponCells)
            {
                if (equippedWeapon == null
                    || equippedWeapon.itemInventoryData == null)
                    continue;
                _currentInventoryMass += equippedWeapon.itemInventoryData.ItemMass;
            }
        }

        private void LoadData()
        {
            _inventorySaveData = _preferenceManager.LoadValue<InventorySaveData>();
            _inventorySaveData.DataReInitialize(_parameters.DefaultInventoryCellsCount,
                _parameters.EquippedWeaponsCount);

            if (!_inventorySaveData.IsAddedContentEarly)
            {
                foreach (var constantWeapon in _parameters.ConstantWeaponCellParametersArray)
                {
                    if (constantWeapon.ConstantWeaponInventoryData.ItemType != ItemType.Weapon
                        || _inventorySaveData.EquippedWeaponCells[constantWeapon.WeaponCellIndex].itemInventoryData != null)
                        continue;

                    _inventorySaveData.EquippedWeaponCells[constantWeapon.WeaponCellIndex] =
                        new InventoryItemCellData(constantWeapon.ConstantWeaponInventoryData, 1);
                    
                    if (constantWeapon.IsLockWeaponCell)
                        _inventorySaveData.EquippedWeaponCells[constantWeapon.WeaponCellIndex].SetLock();
                }

                foreach (var bagCellData in _parameters.BagCellParametersArray)
                {
                    TryAddItemInCurrentCell(bagCellData.BagCellIndex, bagCellData.BagInventoryData, 1,
                        out InventoryItemCellData restOfItemsCell);
                }

                SaveData();
            }
        }

        private void SaveData()
        {
            _preferenceManager.SaveValue(_inventorySaveData);
        }
    }
}