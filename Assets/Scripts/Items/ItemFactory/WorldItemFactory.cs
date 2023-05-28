using UnityEngine;
using Zenject;


public class WorldItemFactory : IItemFactory<ItemWorldObjectControl>
{
    private readonly DiContainer container;
    private readonly ItemCollectionData itemCollectionData;
    
    
    public WorldItemFactory(DiContainer container, ItemCollectionData itemCollectionData)
    {
        this.container = container;
        this.itemCollectionData = itemCollectionData;
    }
    
    
    public ItemWorldObjectControl Create(ItemData prototypePrefab, Transform parent, Vector3 instancePosition)
    {
        ItemWorldObjectControl itemObject = CreateObject(parent);
        itemObject.Initialize(prototypePrefab);
        return itemObject;
    }
    
    private ItemWorldObjectControl CreateObject(Transform parent)
    {
        return Object
            .Instantiate(itemCollectionData.ItemWorldObjectControlPrefab, parent)
            .GetComponent<ItemWorldObjectControl>();
    }
}
