namespace Arenar
{
    public interface IWeapon
    {
        float Damage { get; }
        
        ItemInventoryData ItemInventoryData { get; }
        WeaponType WeaponType { get; }


        void InitializeWeapon(ItemInventoryData itemInventoryData);
    }
}