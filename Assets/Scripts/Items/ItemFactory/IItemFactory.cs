using UnityEngine;


namespace Arenar
{
    public interface IItemFactory<TItem>
    {
         TItem Create(ItemData prototypePrefab, int count, Transform parent, Vector3 instancePosition);
     }
}