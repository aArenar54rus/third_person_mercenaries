using System;
using System.Collections.Generic;


namespace Arenar.Services.InventoryService
{
    public class InventoryService : IInventoryService
    {
        public event Action OnUpdateInventoryData;
        
        
        private Dictionary<int, InventoryItemData> _inventoryItemDatas;
        private float _currentInventoryMass = 0.0f;
        private InventoryOptionsSOData _inventoryOptionsSoData;


        public bool IsMassOverbalance =>
            _currentInventoryMass > InventoryMassMax;

        public int InventoryCellsCount =>
            _inventoryOptionsSoData.DefaultInventoryCellsCount;

        public float InventoryMass =>
            _currentInventoryMass;

        public int InventoryMassMax =>
            _inventoryOptionsSoData.DefaultMassMax;
        
        
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
            foreach (var inventoryItemData in _inventoryItemDatas)
            {
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
            InventoryItemData inventoryItemData = GetInventoryItemDataByCellIndex(cellIndex);

            if (!itemData.CanStack)
            {
                if (inventoryItemData.itemData == null)
                {
                    inventoryItemData.itemData = itemData;
                    inventoryItemData.elementsCount = count;
                    CalculateMass();
                    restOfItems = null;
                    
                    OnUpdateInventoryData?.Invoke();
                    return true;
                }

                restOfItems = new InventoryItemData(itemData, count);
                return false;
            }

            if (inventoryItemData.itemData.Id != itemData.Id
                || inventoryItemData.StackIsFull)
            {
                restOfItems = new InventoryItemData(itemData, count);
                return false;
            }

            inventoryItemData.elementsCount += count;
            count = inventoryItemData.elementsCount - inventoryItemData.itemData.StackCountMax;
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
            throw new NotImplementedException();
        }

        private void Initialize()
        {
            _inventoryItemDatas = new Dictionary<int, InventoryItemData>(InventoryCellsCount);
            for (int i = 0; i < InventoryCellsCount; i++)
                _inventoryItemDatas.Add(i, new InventoryItemData(null, 0));

            CalculateMass();
        }
        
        private InventoryItemData GetInventoryItemDataByCellIndex(int cellIndex)
        {
            return _inventoryItemDatas[cellIndex];
        }
        
        private bool TryAddInFreeCell(ItemData itemData, int count, out InventoryItemData restOfItems)
        {
            foreach (var inventoryItemData in _inventoryItemDatas)
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
            foreach (var inventoryItemData in _inventoryItemDatas)
            {
                if (inventoryItemData.Value == null)
                    continue;

                _currentInventoryMass +=
                    inventoryItemData.Value.elementsCount * inventoryItemData.Value.itemData.ItemMass;
            }
        }
    }
}