using Arenar.Services.InventoryService;
using UnityEngine;


namespace Arenar.Character
{
    public interface IEquipItemFactory<TItem>
    {
        TItem Create(InventoryItemData equippedWeapon, Transform handOwner);
    }
}