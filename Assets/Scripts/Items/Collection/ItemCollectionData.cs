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


        public ItemData GetItemByIndex(int index)
        {
            return gameItems[index];
        }
    }
}