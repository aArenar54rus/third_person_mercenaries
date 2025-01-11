using Arenar.Character;
using System.Collections.Generic;


namespace Arenar
{
    public interface IWeapon
    {
        ICharacterEntity WeaponOwner { get; }
        float Damage { get; }
        ItemInventoryData ItemInventoryData { get; }
        WeaponType WeaponType { get; }
        float TimeBetweenShots { get; }


        void InitializeWeapon(ItemInventoryData itemInventoryData, List<IEquippedItemComponent> itemComponents);
    }
}