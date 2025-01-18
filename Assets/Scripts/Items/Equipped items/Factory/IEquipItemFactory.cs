using Arenar.Services.InventoryService;
using UnityEngine;


namespace Arenar.Items
{
    public interface IEquipItemFactory<TItem>
    {
        TItem Create(ItemInventoryData itemInventoryData);
    }
}