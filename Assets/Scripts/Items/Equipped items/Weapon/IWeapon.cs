using Arenar.Character;

namespace Arenar
{
    public interface IWeapon
    {
        ICharacterEntity WeaponOwner { get; }
        float Damage { get; }
        ItemInventoryData ItemInventoryData { get; }
        WeaponType WeaponType { get; }


        void InitializeWeapon(ItemInventoryData itemInventoryData);
    }
}