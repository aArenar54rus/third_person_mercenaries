using UnityEngine;


namespace Arenar
{
    public interface IItemFactory<TItem>
    {
         TItem Create(ItemInventoryData prototypePrefab, int count, Transform parent, Vector3 instancePosition);
     }
}