using UnityEngine;


public interface IItemFactory<TItem>
{
    TItem Create(ItemData prototypePrefab, Transform parent, Vector3 instancePosition);
}
