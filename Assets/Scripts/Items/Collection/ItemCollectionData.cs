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
        private SerializableDictionary<int, ItemInventoryData> gameItems;

        
        public InteractableElement ItemWorldObjectControlPrefab =>
            itemWorldObjectControlPrefab;

        public ItemRarityColorData ItemRarityColorData =>
            itemRarityColorData;


        public ItemInventoryData GetItemByIndex(int index)
        {
            return gameItems[index];
        }
    }
}