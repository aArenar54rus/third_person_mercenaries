using UnityEngine;


[CreateAssetMenu(menuName = "Items/Item Collection Data")]
public class ItemCollectionData : ScriptableObject
{
    [SerializeField] private ItemWorldObjectControl itemWorldObjectControlPrefab = default;
    
    [Space(10)]
    [SerializeField] private MoneyItemData moneyItemData;


    public ItemWorldObjectControl ItemWorldObjectControlPrefab =>
        itemWorldObjectControlPrefab;
    
    public MoneyItemData MoneyItemData => moneyItemData;
}
