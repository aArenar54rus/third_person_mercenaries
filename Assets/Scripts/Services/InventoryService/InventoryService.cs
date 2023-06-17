using System;
using System.Collections.Generic;


namespace Arenar.Services.InventoryService
{
    public class InventoryService : IInventoryService
    {
        public event Action OnUpdateInventoryData;
        
        
        private Dictionary<int, InventoryItemData> _inventoryItemDataCells;
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


        public InventoryService(InventoryOptionsSOData.Parameters parameters)
        {
            _parameters = parameters;
            Initialize();
        }


        public InventoryItemData GetInventoryItemData(int cellIndex)
        {
            return _inventoryItemDataCells[cellIndex];
        }

        public bool TryAddItem(ItemData itemData, int count, out InventoryItemData restOfItems)
        {
            // first, check mass
            if (itemData.ItemMass * count > InventoryMassMax - InventoryMass)
            {
                restOfItems = new InventoryItemData(itemData, count);
                return false;
            }
            
            if (!itemData.CanStack)
                return TryAddInFreeCell(itemData, count, out restOfItems);

            //check cells with same item
            foreach (var inventoryItemData in _inventoryItemDataCells)
            {
                if (inventoryItemData.Value.itemData == null)
                    continue;
                
                if (inventoryItemData.Value.itemData.Id != itemData.Id
                    || inventoryItemData.Value.StackIsFull)
                    continue;

                inventoryItemData.Value.elementsCount += count;
                count = inventoryItemData.Value.elementsCount - inventoryItemData.Value.itemData.StackCountMax;
                CalculateMass();
                OnUpdateInventoryData?.Invoke();

                if (count > 0)
                    continue;
                
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
                    
                    OnUpdateInventoryData?.Invoke();
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
            OnUpdateInventoryData?.Invoke();

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
            OnUpdateInventoryData?.Invoke();
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
            
            foreach (var dataCell in _inventoryItemDataCells)
            {
                if (dataCell.Value.itemData.Id != itemData.Id)
                    continue;

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
            OnUpdateInventoryData?.Invoke();
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
            foreach (var dataCell in _inventoryItemDataCells)
            {
                if (dataCell.Value.itemData.Id != itemIndex)
                    continue;

                neededItemData ??= dataCell.Value.itemData;
                
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
            OnUpdateInventoryData?.Invoke();
            restOfItems = new InventoryItemData(neededItemData, counter);
            return true;
        }

        private void Initialize()
        {
            _inventoryItemDataCells = new Dictionary<int, InventoryItemData>(InventoryCellsCount);
            for (int i = 0; i < InventoryCellsCount; i++)
                _inventoryItemDataCells.Add(i, new InventoryItemData(null, 0));

            CalculateMass();
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
                    
                OnUpdateInventoryData?.Invoke();
                    
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
        }
    }
}