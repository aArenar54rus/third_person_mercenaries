using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Arenar.Services.InventoryService
{
    public class InventoryService : IInventoryService
    {
        public event Action OnUpdateInventoryData;
        
        
        private float _currentInventoryMass = 0.0f;
        private InventoryOptionsSOData _inventoryOptionsSoData;

        
        public Dictionary<int, InventoryItemData> InventoryItemDatas { get; private set; }

        public bool IsMassOverbalance =>
            _currentInventoryMass > InventoryMassMax;
        
        public bool IsInventoryFull { get; }

        public int InventoryCellsCount =>
            _inventoryOptionsSoData.DefaultInventoryCellsCount;

        public float InventoryMass =>
            _currentInventoryMass;

        public int InventoryMassMax =>
            _inventoryOptionsSoData.DefaultMassMax;
        
        
        public void AddItem(InventoryItemData inventoryItemData)
        {
            throw new System.NotImplementedException();
        }

        public bool TryRemoveItem(InventoryItemData inventoryItemData)
        {
            throw new System.NotImplementedException();
        }

        private void Initialize()
        {
            InventoryItemDatas = new Dictionary<int, InventoryItemData>(InventoryCellsCount);
            for (int i = 0; i < InventoryCellsCount; i++)
                InventoryItemDatas.Add(i, new InventoryItemData(null, 0));

            CalculateMass();
        }

        private void CalculateMass()
        {
            _currentInventoryMass = 0.0f;
            foreach (var inventoryItemData in InventoryItemDatas)
            {
                if (inventoryItemData.Value == null)
                    continue;

                _currentInventoryMass +=
                    inventoryItemData.Value.elementsCount * inventoryItemData.Value.itemData.ItemMass;
            }
        }
    }
}