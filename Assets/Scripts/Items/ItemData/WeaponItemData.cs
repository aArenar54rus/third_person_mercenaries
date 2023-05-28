public class WeaponItemData : ItemData
{
    public override bool CanStack => false;
    public override ItemType ItemType => ItemType.Weapon;
}
