using UnityEngine;
using Zenject;

namespace Arenar
{
    public class WorldItemFactory : IItemFactory<ItemInteractableElement>
    {
        private readonly DiContainer container;
        private readonly ItemCollectionData itemCollectionData;


        public WorldItemFactory(DiContainer container, ItemCollectionData itemCollectionData)
        {
            this.container = container;
            this.itemCollectionData = itemCollectionData;
        }


        public ItemInteractableElement Create(ItemData prototypePrefab, int count, Transform parent, Vector3 instancePosition)
        {
            ItemInteractableElement itemObject = CreateObject(parent);
            itemObject.SetItem(prototypePrefab, count);
            return itemObject;
        }

        private ItemInteractableElement CreateObject(Transform parent)
        {
            return Object
                .Instantiate(itemCollectionData.ItemWorldObjectControlPrefab, parent)
                .GetComponent<ItemInteractableElement>();
        }
    }
}