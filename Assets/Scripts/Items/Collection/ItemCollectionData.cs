using UnityEngine;


namespace Arenar
{
    [CreateAssetMenu(menuName = "Items/Item Collection Data")]
    public class ItemCollectionData : ScriptableObject
    {
        [SerializeField] private InteractableElement itemWorldObjectControlPrefab = default;


        public InteractableElement ItemWorldObjectControlPrefab =>
            itemWorldObjectControlPrefab;
    }
}