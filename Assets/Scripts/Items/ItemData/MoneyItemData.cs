using UnityEngine;


[CreateAssetMenu(menuName = "Items/ItemData/Money Data")]
public class MoneyItemData : ItemData
{
    public override ItemType ItemType => ItemType.Money;
}
