using UnityEngine;


namespace Arenar
{
    [CreateAssetMenu(menuName = "Items/Item Collection Data")]
    public class ItemCollectionData : ScriptableObject
    {
        [SerializeField] private InteractableElement itemWorldObjectControlPrefab = default;
        [SerializeField] private ItemRarityColorData itemRarityColorData = default;


        public InteractableElement ItemWorldObjectControlPrefab =>
            itemWorldObjectControlPrefab;

        public ItemRarityColorData ItemRarityColorData =>
            itemRarityColorData;
    }
}