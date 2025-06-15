using System;
using System.Collections.Generic;
using Arenar.PreferenceSystem;
using UnityEngine;


namespace Arenar.Services.InventoryService
{
    public class InventoryService : IInventoryService
    {
        public event Action<List<int>> OnUpdateInventoryCells;
        public event Action<ItemClothType> OnUpdateEquippedClothItemCell;
        public event Action OnUpdateEquippedWeaponItem;
        
        
        private InventoryOptionsSOData.Parameters _parameters;
        private ItemCollectionData _itemCollectionData;

        private IPreferenceManager _preferenceManager;
        private InventorySaveData _inventorySaveData;
        
        private InventoryCellData _equippedMeleeWeapon;
        private InventoryCellData[] _equippedFirearmWeapons;
        private Dictionary<ItemClothType, InventoryCellData> equippedClothItems;
        private Dictionary<int, InventoryCellData> _inventoryCells;
        
        private float currentInventoryMass = 0.0f;


        public bool IsMassOverbalance =>
            currentInventoryMass > InventoryMassMax;

        public int InventoryCellsCount =>
            _parameters.DefaultInventoryCellsCount;

        public float InventoryMass =>
            currentInventoryMass;

        public int InventoryMassMax =>
            _parameters.DefaultMassMax;
        
        public Dictionary<int, InventoryCellData> InventoryCells => _inventoryCells;


        public InventoryService(InventoryOptionsSOData.Parameters parameters,
                                ItemCollectionData itemCollectionData,
                                IPreferenceManager preferenceManager)
        {
            _parameters = parameters;
            _preferenceManager = preferenceManager;
            _itemCollectionData = itemCollectionData;
            
            Initialize();
        }
        
        
        public InventoryCellData GetInventoryItemDataByCellIndex(int cellIndex)
            => _inventoryCells[cellIndex];
        
        public InventoryCellData GetEquippedMeleeWeapon()
            => _equippedMeleeWeapon;

        public InventoryCellData[] GetEquippedFirearmWeapons()
            => _equippedFirearmWeapons;

        public InventoryCellData GetEquippedCloth(ItemClothType itemClothType)
        {
            if (equippedClothItems.ContainsKey(itemClothType))
            {
                Debug.LogError($"Unknown cloth type {itemClothType}!");
                return null;
            }
            
            return equippedClothItems[itemClothType];
        }
        
        public void EquipMeleeWeaponFromBag(int bagItemIndex)
        {
            if (_inventoryCells[bagItemIndex].itemData == null)
            {
                Debug.LogError($"Bag is empty. Index: {bagItemIndex}!");
                return;
            }

            (_equippedMeleeWeapon, _inventoryCells[bagItemIndex]) = (_inventoryCells[bagItemIndex], _equippedMeleeWeapon);
            
            CalculateMass();
            SaveData();
            
            List<int> indexes = new List<int>();
            indexes.Add(bagItemIndex);
            OnUpdateInventoryCells?.Invoke(indexes);
            OnUpdateEquippedWeaponItem?.Invoke();
        }
        
        public bool TryAddItemsInBag(ItemData itemInventoryData, int count, out InventoryCellData restOfCell)
        {
            // first, check mass
            if (itemInventoryData.ItemMass * count > InventoryMassMax - InventoryMass)
            {
                restOfCell = new InventoryCellData(itemInventoryData, count);
                return false;
            }

            if (!itemInventoryData.CanStack)
                return TryAddInFreeCell(itemInventoryData, count, out restOfCell);

            //check cells with same item
            List<int> updatedCellIndexes = new List<int>();
            for (int i = 0; i <_inventoryCells.Count; i++)
            {
                if (_inventoryCells[i].itemData == null)
                    continue;
                
                if (_inventoryCells[i].itemData.Id != itemInventoryData.Id
                    || _inventoryCells[i].StackIsFull)
                    continue;

                _inventoryCells[i].ElementsCount += count;
                count = _inventoryCells[i].ElementsCount
                    - _inventoryCells[i].itemData.StackCountMax;
                
                updatedCellIndexes.Add(i);
                
                if (count > 0)
                    continue;
                
                CalculateMass();
                OnUpdateInventoryCells?.Invoke(new List<int>(updatedCellIndexes));
                
                restOfCell = null;
                return true;
            }
            
            SaveData();
            
            return TryAddInFreeCell(itemInventoryData, count, out restOfCell);
        }

        public bool TryAddItemInCurrentCell(int cellIndex,
                                            ItemData itemInventoryData,
                                            int count,
                                            out InventoryCellData restOfCell)
        {
            InventoryCellData inventoryCellData = GetInventoryItemDataByCellIndex(cellIndex); 

            if (!itemInventoryData.CanStack)
            {
                if (inventoryCellData.itemData == null)
                {
                    inventoryCellData.itemData = itemInventoryData;
                    inventoryCellData.ElementsCount = 1;
                    restOfCell = null;
                    
                    CalculateMass();
                    SaveData();
                    
                    OnUpdateInventoryCells?.Invoke(new List<int>(cellIndex));
                    return true;
                }

                restOfCell = new InventoryCellData(itemInventoryData, count);
                return false;
            }

            if (inventoryCellData.itemData == null)
            {
                restOfCell = new InventoryCellData(itemInventoryData, count);
                _inventoryCells[cellIndex] = restOfCell;
                
                CalculateMass();
                SaveData();
                OnUpdateInventoryCells?.Invoke(new List<int>(1){ cellIndex });
                return true;
            }

            inventoryCellData.ElementsCount += count;
            count = inventoryCellData.ElementsCount - inventoryCellData.itemData.StackCountMax;
            
            CalculateMass();
            SaveData();
            OnUpdateInventoryCells?.Invoke(new List<int>(1){ cellIndex });

            if (count > 0)
            {
                restOfCell = new InventoryCellData(itemInventoryData, count);
                return false;
            }
            
            restOfCell = null;
            return true;
        }

        public void RemoveItemFromCell(int cellIndex, int count, out InventoryCellData restOfCell)
        {
            var dataCell = GetInventoryItemDataByCellIndex(cellIndex);
            if (dataCell.ElementsCount < count)
            {
                restOfCell = null;
                return;
            }
            
            restOfCell = new InventoryCellData(dataCell.itemData, count);
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
            
            foreach (var inventoryCell in _inventoryCells.Values)
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

        public bool TryRemoveItems(ItemData itemInventoryData, int neededCount, out InventoryCellData restOfCell)
        {
            if (!IsEnoughItems(itemInventoryData.Id, neededCount))
            {
                restOfCell = null;
                return false;
            }

            int counter = 0;
            List<int> changedCellsIndexes = new List<int>();
            
            for (int i = 0; i < _inventorySaveData.InventoryBagCells.Length; i++)
            {
                if (_inventoryCells[i].itemData.Id != itemInventoryData.Id)
                    continue;

                changedCellsIndexes.Add(i);
                if (_inventoryCells[i].ElementsCount <= neededCount)
                {
                    counter += _inventoryCells[i].ElementsCount;
                    neededCount -= _inventoryCells[i].ElementsCount;
                    _inventoryCells[i].itemData = null;
                    _inventoryCells[i].ElementsCount = 0;

                    if (neededCount == 0)
                        break;
                }
                else
                {
                    counter += neededCount;
                    _inventoryCells[i].ElementsCount -= neededCount;
                    break;
                }
            }

            CalculateMass();
            SaveData();
            OnUpdateInventoryCells?.Invoke(changedCellsIndexes);
            restOfCell = new InventoryCellData(itemInventoryData, counter);
            return true;
        }

        public bool TryRemoveItems(int itemIndex, int neededCount, out InventoryCellData restOfCell)
        {
            if (!IsEnoughItems(itemIndex, neededCount))
            {
                restOfCell = null;
                return false;
            }

            int counter = 0;

            ItemData neededItemInventoryData = null;
            List<int> changedCellsIndexes = new List<int>();
            
            for (int i = 0; i < _inventoryCells.Count; i++)
            {
                if (_inventoryCells[i].itemData.Id != itemIndex)
                    continue;
                
                changedCellsIndexes.Add(i);
                neededItemInventoryData = _inventoryCells[i].itemData;
                if (_inventoryCells[i].ElementsCount <= neededCount)
                {
                    counter += _inventoryCells[i].ElementsCount;
                    neededCount -= _inventoryCells[i].ElementsCount;
                    _inventoryCells[i].itemData = null;
                    _inventoryCells[i].ElementsCount = 0;

                    if (neededCount == 0)
                        break;
                }
                else
                {
                    counter += neededCount;
                    _inventoryCells[i].ElementsCount -= neededCount;
                    break;
                }
            }

            CalculateMass();
            SaveData();
            OnUpdateInventoryCells?.Invoke(changedCellsIndexes);
            restOfCell = new InventoryCellData(neededItemInventoryData, counter);
            return true;
        }

        private void Initialize()
        {
            InitializeInventory();
            
            /*_inventoryItemDataCells = new Dictionary<int, InventoryItemCellData>(InventoryCellsCount);
            for (int i = 0; i < InventoryCellsCount; i++)
                _inventoryItemDataCells.Add(i, new InventoryItemCellData(null, 0));

            _equippedClothItems = new Dictionary<ItemClothType, InventoryItemCellData>();
            _equippedClothItems.Add(ItemClothType.Head, new InventoryItemCellData(null, 0));
            _equippedClothItems.Add(ItemClothType.Body, new InventoryItemCellData(null, 0));
            _equippedClothItems.Add(ItemClothType.Hands, new InventoryItemCellData(null, 0));
            _equippedClothItems.Add(ItemClothType.Foots, new InventoryItemCellData(null, 0));#1#*/
            
            CalculateMass();
        }

        private bool TryAddInFreeCell(ItemData itemInventoryData, int count, out InventoryCellData restOfCell)
        {
            for(int i = 0; i < _inventorySaveData.InventoryBagCells.Length; i++)
            {
                if (_inventoryCells[i].itemData != null)
                    continue;
                    
                _inventoryCells[i].itemData = itemInventoryData;
                _inventoryCells[i].ElementsCount = count;
                CalculateMass();
                SaveData();
                restOfCell = null;
                    
                OnUpdateInventoryCells?.Invoke(new List<int>(1){ i });

                return true;
            }

            restOfCell = new InventoryCellData(itemInventoryData, count);
            return false;
        }

        private void CalculateMass()
        {
            currentInventoryMass = 0.0f;
            
            if (_parameters.IsMathMass)
            {

                foreach (var inventoryItemData in _inventoryCells.Values)
                {
                    if (inventoryItemData == null || inventoryItemData.itemData == null)
                        continue;

                    currentInventoryMass += inventoryItemData.ElementsCount * inventoryItemData.itemData.ItemMass;
                }
            }

            if (_parameters.IsMathMassEquipped)
            {
                foreach (var equippedClothItem in equippedClothItems.Values)
                {
                    if (equippedClothItem == null || equippedClothItem.itemData == null)
                        continue;

                    currentInventoryMass += equippedClothItem.itemData.ItemMass;
                }

                foreach (var equippedWeapon in _equippedFirearmWeapons)
                {
                    if (equippedWeapon == null || equippedWeapon.itemData == null)
                        continue;

                    currentInventoryMass += equippedWeapon.itemData.ItemMass;
                }
                
                if (_equippedMeleeWeapon != null && _equippedMeleeWeapon.itemData != null)
                    currentInventoryMass += _equippedMeleeWeapon.itemData.ItemMass;
            }
        }

        private void InitializeInventory()
        {
            Initialize_LoadSaveData();

            if (_inventorySaveData.EquippedMeleeWeaponsCell.itemId >= 0)
            {
                ItemData meleeWeaponData = _itemCollectionData.GetItemByIndex(
                    _inventorySaveData.EquippedMeleeWeaponsCell.itemId
                );

                _equippedMeleeWeapon = new InventoryCellData(meleeWeaponData, _inventorySaveData.EquippedMeleeWeaponsCell.itemCount);
            }
            else
            {
                _equippedMeleeWeapon = new InventoryCellData(null, 0);
            }

            _equippedFirearmWeapons = new InventoryCellData[_parameters.EquippedWeaponsCount];
            for (int i = 0; i < _parameters.EquippedWeaponsCount; i++)
            {
                if (_inventorySaveData.FirearmWeapons[i].itemId < 0)
                {
                    _equippedFirearmWeapons[i] = new InventoryCellData(null, 0);
                    continue;
                }

                ItemData weaponData = _itemCollectionData.GetItemByIndex(_inventorySaveData.FirearmWeapons[i].itemId);
                _equippedFirearmWeapons[i] = new InventoryCellData(weaponData, 1);
            }

            _inventoryCells = new Dictionary<int, InventoryCellData>(_parameters.DefaultInventoryCellsCount);
            for (int i = 0; i < _parameters.EquippedWeaponsCount; i++) {
                ItemData bagData = null;
                
                var saveBagCell = _inventorySaveData.InventoryBagCells[i];
                if (saveBagCell.itemId > -1)
                    bagData = _itemCollectionData.GetItemByIndex(saveBagCell.itemId);
                
                _inventoryCells[i] = new InventoryCellData(bagData, bagData == null ? 0 : saveBagCell.itemCount);
            }
        }
        
        private void Initialize_LoadSaveData()
        {
            _inventorySaveData = _preferenceManager.LoadValue<InventorySaveData>();

            if (_inventorySaveData.IsAddedContentEarly)
                return;

            InitializeNewInventorySaveData();
            
            // load melee weapon
            var meleeWeaponId = _parameters.StarterMeleeWeaponId;
            if (meleeWeaponId >= 0
                && _itemCollectionData.IsCurrectItemType(meleeWeaponId, ItemType.MeleeWeapon))
            {
                ItemData itemData = _itemCollectionData.GetItemByIndex(meleeWeaponId);
                var data = CreateInventoryCellData(itemData, 1, 1);
                _inventorySaveData.EquippedMeleeWeaponsCell.UpdateData(data);
            }
            else
            {
                _inventorySaveData.EquippedMeleeWeaponsCell.UpdateData(null);
            }

            for (int i = 0; i < _parameters.EquippedWeaponsCount; i++) {
                if (i >= _parameters.ConstantWeaponCellParametersArray.Length) {
                    _inventorySaveData.FirearmWeapons[i].UpdateData(null);
                    continue;
                }

                var firearmWeaponParameters = _parameters.ConstantWeaponCellParametersArray[i];
                int itemId = firearmWeaponParameters.ConstantWeaponId;
                if (itemId < 0 || !_itemCollectionData.IsCurrectItemType(itemId, ItemType.FirearmWeapon))
                {
                    _inventorySaveData.FirearmWeapons[i].UpdateData(null);
                    continue;
                }

                ItemData itemData = _itemCollectionData.GetItemByIndex(itemId);
                var data = CreateInventoryCellData(itemData, 1, 1);
            
                _inventorySaveData.FirearmWeapons[i].UpdateData(data);
            }

            foreach (var bagCellData in _parameters.BagCellParametersArray) {
                ItemData itemData = _itemCollectionData.GetItemByIndex(bagCellData.ItemId);
                var data = CreateInventoryCellData(itemData, bagCellData.ItemCount, 1);
                _inventorySaveData.InventoryBagCells[bagCellData.ItemBagCellIndex].UpdateData(data);
            }

            _inventorySaveData.IsAddedContentEarly = true;
            _preferenceManager.SaveValue(_inventorySaveData);


            
            void InitializeNewInventorySaveData()
            {
                _inventorySaveData.EquippedMeleeWeaponsCell = new InventoryCellSaveData();
            
                _inventorySaveData.FirearmWeapons = new InventoryCellSaveData[_parameters.EquippedWeaponsCount];
                for (int i = 0; i < _parameters.EquippedWeaponsCount; i++)
                    _inventorySaveData.FirearmWeapons[i] = new InventoryCellSaveData();
            
                _inventorySaveData.InventoryBagCells = new InventoryCellSaveData[_parameters.DefaultInventoryCellsCount];
                for (int i = 0; i < _parameters.DefaultInventoryCellsCount; i++)
                    _inventorySaveData.InventoryBagCells[i] = new InventoryCellSaveData();

                _inventorySaveData.IsAddedContentEarly = false;
            }
        }

        private InventoryCellData CreateInventoryCellData(ItemData itemData = null, int count = 1, int itemLevel = 1) {
            return new() {
                    itemData = itemData,
                    ElementsCount = itemData == null ? 0 : count,
                    ItemLevel = itemData == null ? 0 :itemLevel,
            };
        }
        
        private void SaveData()
        {
            // melee weapon
            _inventorySaveData.EquippedMeleeWeaponsCell.UpdateData(_equippedMeleeWeapon);

            // firearm weapon
            for (int i = 0; i < _equippedFirearmWeapons.Length; i++)
            {
                _inventorySaveData.FirearmWeapons[i]
                    .UpdateData(_equippedFirearmWeapons[i]);
            }
            
            _preferenceManager.SaveValue(_inventorySaveData);
        }
    }
}