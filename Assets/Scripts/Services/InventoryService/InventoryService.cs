using System;
using System.Collections.Generic;
using UnityEngine;


namespace Arenar.Services.InventoryService
{
    public class InventoryService : IInventoryService
    {
        public event Action<List<int>> OnUpdateInventoryCells;
        public event Action<ItemClothType> OnUpdateEquippedClothItemCell;
        public event Action OnUpdateEquippedWeaponItem;
        
        
        private Dictionary<int, InventoryItemData> _inventoryItemDataCells;
        private Dictionary<ItemClothType, InventoryItemData> _equippedClothItems;
        private InventoryItemData _equippedWeapon;
        
        private float _currentInventoryMass = 0.0f;
        private InventoryOptionsSOData.Parameters _parameters;


        public bool IsMassOverbalance =>
            _currentInventoryMass > InventoryMassMax;

        public int InventoryCellsCount =>
            _parameters.DefaultInventoryCellsCount;

        public float InventoryMass =>
            _currentInventoryMass;

        public int InventoryMassMax =>
            _parameters.DefaultMassMax;

        public Dictionary<ItemClothType, InventoryItemData> EquippedClothItems =>
            _equippedClothItems;


        public InventoryService(InventoryOptionsSOData.Parameters parameters)
        {
            _parameters = parameters;
            Initialize();
        }
        
        public InventoryItemData GetInventoryItemData(int cellIndex)
        {
            return _inventoryItemDataCells[cellIndex];
        }

        public InventoryItemData GetEquippedWeapon()
        {
            return _equippedWeapon;
        }

        public InventoryItemData GetEquippedCloth(ItemClothType itemClothType)
        {
            if (_equippedClothItems.ContainsKey(itemClothType))
            {
                Debug.LogError($"Unknown cloth type {itemClothType}!");
                return null;
            }
            
            return _equippedClothItems[itemClothType];
        }

        public bool TryAddItems(ItemData itemData, int count, out InventoryItemData restOfItems)
        {
            // first, check mass
            if (itemData.ItemMass * count > InventoryMassMax - InventoryMass)
            {
                restOfItems = new InventoryItemData(itemData, count);
                return false;
            }

            if (itemData.ItemType == ItemType.Weapon
                && _equippedWeapon.itemData == null)
            {
                _equippedWeapon.itemData = itemData;
                _equippedWeapon.elementsCount = 1;
                restOfItems = null;
                CalculateMass();
                OnUpdateEquippedWeaponItem?.Invoke();
                return true;
            }
            
            if (!itemData.CanStack)
                return TryAddInFreeCell(itemData, count, out restOfItems);

            //check cells with same item
            List<int> updatedCellIndexes = new List<int>();
            foreach (var inventoryItemData in _inventoryItemDataCells)
            {
                if (inventoryItemData.Value.itemData == null)
                    continue;
                
                if (inventoryItemData.Value.itemData.Id != itemData.Id
                    || inventoryItemData.Value.StackIsFull)
                    continue;

                inventoryItemData.Value.elementsCount += count;
                count = inventoryItemData.Value.elementsCount - inventoryItemData.Value.itemData.StackCountMax;
                updatedCellIndexes.Add(inventoryItemData.Key);
                
                if (count > 0)
                    continue;
                
                CalculateMass();
                OnUpdateInventoryCells?.Invoke(new List<int>(updatedCellIndexes));
                
                restOfItems = null;
                return true;
            }
            
            return TryAddInFreeCell(itemData, count, out restOfItems);
        }

        public bool TryAddItemInCurrentCell(int cellIndex,
                                            ItemData itemData,
                                            int count,
                                            out InventoryItemData restOfItems)
        {
            InventoryItemData dataCell = _inventoryItemDataCells[cellIndex];

            if (!itemData.CanStack)
            {
                if (dataCell.itemData == null)
                {
                    dataCell.itemData = itemData;
                    dataCell.elementsCount = count;
                    CalculateMass();
                    restOfItems = null;
                    
                    OnUpdateInventoryCells?.Invoke(new List<int>(cellIndex));
                    return true;
                }

                restOfItems = new InventoryItemData(itemData, count);
                return false;
            }

            if (dataCell.itemData.Id != itemData.Id
                || dataCell.StackIsFull)
            {
                restOfItems = new InventoryItemData(itemData, count);
                return false;
            }

            dataCell.elementsCount += count;
            count = dataCell.elementsCount - dataCell.itemData.StackCountMax;
            
            CalculateMass();
            OnUpdateInventoryCells?.Invoke(new List<int>(1){ cellIndex });

            if (count > 0)
            {
                restOfItems = new InventoryItemData(itemData, count);
                return false;
            }
            
            restOfItems = null;
            return true;
        }

        public void RemoveItemFromCell(int cellIndex, int count, out InventoryItemData restOfItems)
        {
            var dataCell =  _inventoryItemDataCells[cellIndex];
            if (dataCell.elementsCount < count)
            {
                restOfItems = null;
                return;
            }
            
            restOfItems = new InventoryItemData(dataCell.itemData, count);
            dataCell.elementsCount -= count;
            if (dataCell.elementsCount == 0)
                dataCell.itemData = null;
            
            CalculateMass();
            OnUpdateInventoryCells?.Invoke(new List<int>(cellIndex));
        }

        public bool IsEnoughItems(ItemData itemData, int neededCount)
        {
            return IsEnoughItems(itemData.Id, neededCount);
        }

        public bool IsEnoughItems(int itemIndex, int neededCount)
        {
            int counter = 0;
            foreach (var dataCell in _inventoryItemDataCells)
            {
                if (dataCell.Value.itemData.Id != itemIndex)
                    continue;

                counter += dataCell.Value.elementsCount;
                if (counter >= neededCount)
                    return true;
            }

            return false;
        }

        public bool TryRemoveItems(ItemData itemData, int neededCount, out InventoryItemData restOfItems)
        {
            if (!IsEnoughItems(itemData.Id, neededCount))
            {
                restOfItems = null;
                return false;
            }

            int counter = 0;
            List<int> changedCellsIndexes = new List<int>();
            
            foreach (var dataCell in _inventoryItemDataCells)
            {
                if (dataCell.Value.itemData.Id != itemData.Id)
                    continue;

                changedCellsIndexes.Add(dataCell.Key);
                if (dataCell.Value.elementsCount <= neededCount)
                {
                    counter += dataCell.Value.elementsCount;
                    neededCount -= dataCell.Value.elementsCount;
                    dataCell.Value.itemData = null;
                    dataCell.Value.elementsCount = 0;

                    if (neededCount == 0)
                        break;
                }
                else
                {
                    counter += neededCount;
                    dataCell.Value.elementsCount -= neededCount;
                    break;
                }
            }

            CalculateMass();
            OnUpdateInventoryCells?.Invoke(changedCellsIndexes);
            restOfItems = new InventoryItemData(itemData, counter);
            return true;
        }

        public bool TryRemoveItems(int itemIndex, int neededCount, out InventoryItemData restOfItems)
        {
            if (!IsEnoughItems(itemIndex, neededCount))
            {
                restOfItems = null;
                return false;
            }

            int counter = 0;

            ItemData neededItemData = null;
            List<int> changedCellsIndexes = new List<int>();
            foreach (var dataCell in _inventoryItemDataCells)
            {
                if (dataCell.Value.itemData.Id != itemIndex)
                    continue;

                neededItemData ??= dataCell.Value.itemData;
                changedCellsIndexes.Add(dataCell.Key);
                if (dataCell.Value.elementsCount <= neededCount)
                {
                    counter += dataCell.Value.elementsCount;
                    neededCount -= dataCell.Value.elementsCount;
                    dataCell.Value.itemData = null;
                    dataCell.Value.elementsCount = 0;

                    if (neededCount == 0)
                        break;
                }
                else
                {
                    counter += neededCount;
                    dataCell.Value.elementsCount -= neededCount;
                    break;
                }
            }

            CalculateMass();
            OnUpdateInventoryCells?.Invoke(changedCellsIndexes);
            restOfItems = new InventoryItemData(neededItemData, counter);
            return true;
        }

        private void Initialize()
        {
            _inventoryItemDataCells = new Dictionary<int, InventoryItemData>(InventoryCellsCount);
            for (int i = 0; i < InventoryCellsCount; i++)
                _inventoryItemDataCells.Add(i, new InventoryItemData(null, 0));

            _equippedClothItems = new Dictionary<ItemClothType, InventoryItemData>();
            _equippedClothItems.Add(ItemClothType.Head, new InventoryItemData(null, 0));
            _equippedClothItems.Add(ItemClothType.Body, new InventoryItemData(null, 0));
            _equippedClothItems.Add(ItemClothType.Hands, new InventoryItemData(null, 0));
            _equippedClothItems.Add(ItemClothType.Foots, new InventoryItemData(null, 0));

            _equippedWeapon = new InventoryItemData(null, 0);

            DownloadInventory();
            
            CalculateMass();
        }

        private void DownloadInventory()
        {
            
        }

        private bool TryAddInFreeCell(ItemData itemData, int count, out InventoryItemData restOfItems)
        {
            foreach (var inventoryItemData in _inventoryItemDataCells)
            {
                if (inventoryItemData.Value.itemData != null)
                    continue;
                    
                inventoryItemData.Value.itemData = itemData;
                inventoryItemData.Value.elementsCount = count;
                CalculateMass();
                restOfItems = null;
                    
                OnUpdateInventoryCells?.Invoke(new List<int>(1){ inventoryItemData.Key });

                return true;
            }

            restOfItems = new InventoryItemData(itemData, count);
            return false;
        }

        private void CalculateMass()
        {
            _currentInventoryMass = 0.0f;
            foreach (var inventoryItemData in _inventoryItemDataCells)
            {
                if (inventoryItemData.Value == null
                    || inventoryItemData.Value.itemData == null)
                    continue;

                _currentInventoryMass +=
                    inventoryItemData.Value.elementsCount * inventoryItemData.Value.itemData.ItemMass;
            }

            foreach (var equippedClothItem in _equippedClothItems)
            {
                if (equippedClothItem.Value == null
                    || equippedClothItem.Value.itemData == null)
                    continue;

                _currentInventoryMass += equippedClothItem.Value.itemData.ItemMass;
            }

            if (_equippedWeapon == null
                || _equippedWeapon.itemData == null)
                return;
            _currentInventoryMass += _equippedWeapon.itemData.ItemMass;
        }
    }
}