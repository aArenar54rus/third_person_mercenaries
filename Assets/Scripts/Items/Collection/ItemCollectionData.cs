using Arenar.Items;
using UnityEngine;


namespace Arenar
{
    [CreateAssetMenu(menuName = "Items/Item Collection Data")]
    public class ItemCollectionData : ScriptableObject
    {
        [SerializeField]
        private InteractableElement itemWorldObjectControlPrefab = default;
        [SerializeField]
        private ItemRarityColorData itemRarityColorData = default;
        [SerializeField]
        private SerializableDictionary<int, ItemData> gameItems;

        
        public InteractableElement ItemWorldObjectControlPrefab =>
            itemWorldObjectControlPrefab;

        public ItemRarityColorData ItemRarityColorData =>
            itemRarityColorData;

        public SerializableDictionary<int, ItemData> GameItems => gameItems;


        public bool IsCurrectItemType(int itemIndex, ItemType type) {
            if (gameItems.ContainsKey(itemIndex)) {
                return gameItems[itemIndex].ItemType == type;
            }

            return false;
        }
        
        public ItemData GetFirst(ItemType type) {
            foreach (var item in gameItems) {
                if (item.Value.ItemType == type)
                    return item.Value;
            }

            return null;
        }

        public ItemData GetItemByIndex(int itemIndex)
        {
            if (gameItems.ContainsKey(itemIndex))
                return gameItems[itemIndex];

            return null;
        }
    }
}