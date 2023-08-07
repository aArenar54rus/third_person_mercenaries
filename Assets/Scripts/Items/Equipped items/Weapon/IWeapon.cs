namespace Arenar
{
    public interface IWeapon
    {
        float Damage { get; }
        
        ItemData ItemData { get; }
        WeaponType WeaponType { get; }


        void InitializeWeapon(ItemData itemData);
    }
}