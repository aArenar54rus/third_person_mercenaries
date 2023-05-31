using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Arenar.Services.InventoryService
{
    public interface IInventoryService
    {
        event Action OnUpdateInventoryData;
        
        
        Dictionary<int, InventoryItemData> InventoryItemDatas { get; }
        
        bool IsMassOverbalance { get; }
        
        bool IsInventoryFull { get; }
        
        int InventoryCellsCount { get; }
        
        float InventoryMass { get; }
        
        int InventoryMassMax { get; }


        void AddItem(InventoryItemData inventoryItemData);

        bool TryRemoveItem(InventoryItemData inventoryItemData);
    }
}
