using UnityEngine;


namespace Arenar
{
    [CreateAssetMenu(menuName = "Items/Item Collection Data")]
    public class ItemCollectionData : ScriptableObject
    {
        [SerializeField] private InteractableElement itemWorldObjectControlPrefab = default;
        
        [Space(10)]
        [SerializeField] private MoneyItemData moneyItemData;


        public InteractableElement ItemWorldObjectControlPrefab =>
            itemWorldObjectControlPrefab;
        
        public MoneyItemData MoneyItemData => moneyItemData;
    }
}