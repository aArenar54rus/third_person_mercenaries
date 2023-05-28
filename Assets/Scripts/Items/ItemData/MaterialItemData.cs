public class MaterialItemData : ItemData
{
    public override bool CanStack => false;
    
    public override ItemType ItemType => ItemType.Material;
}
