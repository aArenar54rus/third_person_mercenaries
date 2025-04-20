using System;
using System.Collections.Generic;
using Arenar.PreferenceSystem;
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

        public int CurrencyMoney
        {
            get => PlayerPrefs.GetInt("Money", 0); 
            set => PlayerPrefs.SetInt("Money", value);
        }


        public InventoryService(InventoryOptionsSOData.Parameters parameters, IPreferenceManager preferenceManager)
        {
            _parameters = parameters;
            _preferenceManager = preferenceManager;
            Initialize();
        }


        public InventoryItemCellData GetInventoryItemDataByCellIndex(int cellIndex)
        {
            return _inventorySaveData.InventoryBagCells[cellIndex];
        }

        public InventoryItemCellData[] GetAllBagItems()
        {
            return _inventorySaveData.InventoryBagCells;
        }
        
        public InventoryItemCellData GetEquippedMeleeWeapon()
        {
            return _inventorySaveData.EquippedMeleeWeaponsCell;
        }

        public InventoryItemCellData[] GetEquippedFirearmWeapons() =>
            _inventorySaveData.EquippedFirearmWeaponCells;

        public InventoryItemCellData GetEquippedCloth(ItemClothType itemClothType)
        {
            if (_equippedClothItems.ContainsKey(itemClothType))
            {
                Debug.LogError($"Unknown cloth type {itemClothType}!");
                return null;
            }
            
            return _equippedClothItems[itemClothType];
        }
        
        public void EquipMeleeWeaponFromBag(int bagItemIndex)
        {
            if (_inventorySaveData.EquippedMeleeWeaponsCell.itemData != null)
            {
                if (!TryAddItemsInBag(_inventorySaveData.EquippedMeleeWeaponsCell.itemData, 1, out InventoryItemCellData _))
                {
                    return;
                }
            }
            
            _inventorySaveData.EquippedMeleeWeaponsCell = _inventorySaveData.InventoryBagCells[bagItemIndex];
            _inventorySaveData.InventoryBagCells[bagItemIndex].itemData = null;
            _inventorySaveData.InventoryBagCells[bagItemIndex].ElementsCount = 0;
            
            CalculateMass();
            SaveData();
            
            List<int> indexes = new List<int>();
            indexes.Add(bagItemIndex);
            OnUpdateInventoryCells?.Invoke(indexes);
            
            OnUpdateEquippedWeaponItem?.Invoke();
        }
        
        public bool TryAddItemsInBag(ItemData itemInventoryData, int count, out InventoryItemCellData restOfItemsCell)
        {
            // first, check mass
            if (itemInventoryData.ItemMass * count > InventoryMassMax - InventoryMass)
            {
                restOfItemsCell = new InventoryItemCellData(itemInventoryData, count);
                return false;
            }

            /*if (itemInventoryData.ItemType == ItemType.FirearmWeapon)
            {
                for (int i = 0; i < _inventorySaveData.EquippedFirearmWeaponCells.Length; i++)
                {
                    _inventorySaveData.EquippedFirearmWeaponCells[i] ??= new InventoryItemCellData();
                    if (_inventorySaveData.EquippedFirearmWeaponCells[i].itemData != null)
                        continue;
                    
                    _inventorySaveData.EquippedFirearmWeaponCells[i].itemData = itemInventoryData;
                    _inventorySaveData.EquippedFirearmWeaponCells[i].ElementsCount = 1;
                    restOfItemsCell = null;
                    CalculateMass();
                    OnUpdateEquippedWeaponItem?.Invoke();
                    return true;
                }
            }*/

            if (!itemInventoryData.CanStack)
                return TryAddInFreeCell(itemInventoryData, count, out restOfItemsCell);

            //check cells with same item
            List<int> updatedCellIndexes = new List<int>();
            for (int i = 0; i < _inventorySaveData.InventoryBagCells.Length; i++)
            {
                if (_inventorySaveData.InventoryBagCells[i].itemData == null)
                    continue;
                
                if (_inventorySaveData.InventoryBagCells[i].itemData.Id != itemInventoryData.Id
                    || _inventorySaveData.InventoryBagCells[i].StackIsFull)
                    continue;

                _inventorySaveData.InventoryBagCells[i].ElementsCount += count;
                count = _inventorySaveData.InventoryBagCells[i].ElementsCount
                    - _inventorySaveData.InventoryBagCells[i].itemData.StackCountMax;
                
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
                                            ItemData itemInventoryData,
                                            int count,
                                            out InventoryItemCellData restOfItemsCell)
        {
            InventoryItemCellData inventoryItemCellData = GetInventoryItemDataByCellIndex(cellIndex); 

            if (!itemInventoryData.CanStack)
            {
                if (inventoryItemCellData.itemData == null)
                {
                    inventoryItemCellData.itemData = itemInventoryData;
                    inventoryItemCellData.ElementsCount = 1;
                    restOfItemsCell = null;
                    
                    CalculateMass();
                    SaveData();
                    
                    OnUpdateInventoryCells?.Invoke(new List<int>(cellIndex));
                    return true;
                }

                restOfItemsCell = new InventoryItemCellData(itemInventoryData, count);
                return false;
            }

            if (inventoryItemCellData.itemData == null)
            {
                restOfItemsCell = new InventoryItemCellData(itemInventoryData, count);
                _inventorySaveData.InventoryBagCells[cellIndex] = restOfItemsCell;
                
                CalculateMass();
                SaveData();
                OnUpdateInventoryCells?.Invoke(new List<int>(1){ cellIndex });
                return true;
            }

            inventoryItemCellData.ElementsCount += count;
            count = inventoryItemCellData.ElementsCount - inventoryItemCellData.itemData.StackCountMax;
            
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
            var dataCell = GetInventoryItemDataByCellIndex(cellIndex);
            if (dataCell.ElementsCount < count)
            {
                restOfItemsCell = null;
                return;
            }
            
            restOfItemsCell = new InventoryItemCellData(dataCell.itemData, count);
            dataCell.ElementsCount -= count;
            if (dataCell.ElementsCount == 0)
                dataCell.itemData = null;
            
            CalculateMass();
            SaveData();
            
            List<int> usedIndexes = new List<int>();
            usedIndexes.Add(cellIndex);
            
            OnUpdateInventoryCells?.Invoke(usedIndexes);
        }

        public bool IsEnoughItems(ItemData itemInventoryData, int neededCount)
        {
            return IsEnoughItems(itemInventoryData.Id, neededCount);
        }

        public bool IsEnoughItems(int itemIndex, int neededCount)
        {
            int counter = 0;
            
            foreach (var inventoryCell in _inventorySaveData.InventoryBagCells)
            {
                if (inventoryCell.itemData == null)
                    continue;
                
                if (inventoryCell.itemData.Id != itemIndex)
                    continue;

                counter += inventoryCell.ElementsCount;
                if (counter >= neededCount)
                    return true;
            }

            return false;
        }

        public bool TryRemoveItems(ItemData itemInventoryData, int neededCount, out InventoryItemCellData restOfItemsCell)
        {
            if (!IsEnoughItems(itemInventoryData.Id, neededCount))
            {
                restOfItemsCell = null;
                return false;
            }

            int counter = 0;
            List<int> changedCellsIndexes = new List<int>();
            
            for (int i = 0; i < _inventorySaveData.InventoryBagCells.Length; i++)
            {
                if (_inventorySaveData.InventoryBagCells[i].itemData.Id != itemInventoryData.Id)
                    continue;

                changedCellsIndexes.Add(i);
                if (_inventorySaveData.InventoryBagCells[i].ElementsCount <= neededCount)
                {
                    counter += _inventorySaveData.InventoryBagCells[i].ElementsCount;
                    neededCount -= _inventorySaveData.InventoryBagCells[i].ElementsCount;
                    _inventorySaveData.InventoryBagCells[i].itemData = null;
                    _inventorySaveData.InventoryBagCells[i].ElementsCount = 0;

                    if (neededCount == 0)
                        break;
                }
                else
                {
                    counter += neededCount;
                    _inventorySaveData.InventoryBagCells[i].ElementsCount -= neededCount;
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

            ItemData neededItemInventoryData = null;
            List<int> changedCellsIndexes = new List<int>();
            for (int i = 0; i < _inventorySaveData.InventoryBagCells.Length; i++)
            {
                if (_inventorySaveData.InventoryBagCells[i].itemData.Id != itemIndex)
                    continue;
                
                changedCellsIndexes.Add(i);
                neededItemInventoryData = _inventorySaveData.InventoryBagCells[i].itemData;
                if (_inventorySaveData.InventoryBagCells[i].ElementsCount <= neededCount)
                {
                    counter += _inventorySaveData.InventoryBagCells[i].ElementsCount;
                    neededCount -= _inventorySaveData.InventoryBagCells[i].ElementsCount;
                    _inventorySaveData.InventoryBagCells[i].itemData = null;
                    _inventorySaveData.InventoryBagCells[i].ElementsCount = 0;

                    if (neededCount == 0)
                        break;
                }
                else
                {
                    counter += neededCount;
                    _inventorySaveData.InventoryBagCells[i].ElementsCount -= neededCount;
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

        private bool TryAddInFreeCell(ItemData itemInventoryData, int count, out InventoryItemCellData restOfItemsCell)
        {
            for(int i = 0; i < _inventorySaveData.InventoryBagCells.Length; i++)
            {
                if (_inventorySaveData.InventoryBagCells[i].itemData != null)
                    continue;
                    
                _inventorySaveData.InventoryBagCells[i].itemData = itemInventoryData;
                _inventorySaveData.InventoryBagCells[i].ElementsCount = count;
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
            foreach (var inventoryItemData in _inventorySaveData.InventoryBagCells)
            {
                if (inventoryItemData == null
                    || inventoryItemData.itemData == null)
                    continue;

                _currentInventoryMass +=
                    inventoryItemData.ElementsCount * inventoryItemData.itemData.ItemMass;
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
            
            foreach (var equippedWeapon in _inventorySaveData.EquippedFirearmWeaponCells)
            {
                if (equippedWeapon == null
                    || equippedWeapon.itemData == null)
                    continue;
                _currentInventoryMass += equippedWeapon.itemData.ItemMass;
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
                    if (constantWeapon.ConstantWeaponData.ItemType != ItemType.FirearmWeapon
                        || _inventorySaveData.EquippedFirearmWeaponCells[constantWeapon.WeaponCellIndex].itemData != null)
                        continue;

                    _inventorySaveData.EquippedFirearmWeaponCells[constantWeapon.WeaponCellIndex] =
                        new InventoryItemCellData(constantWeapon.ConstantWeaponData, 1);
                    
                    if (constantWeapon.IsLockWeaponCell)
                        _inventorySaveData.EquippedFirearmWeaponCells[constantWeapon.WeaponCellIndex].SetLock();
                }

                foreach (var bagCellData in _parameters.BagCellParametersArray)
                {
                    TryAddItemInCurrentCell(
                        bagCellData.ItemBagCellIndex,
                        bagCellData.BagData,
                        bagCellData.ItemCount,
                        out InventoryItemCellData _
                    );
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