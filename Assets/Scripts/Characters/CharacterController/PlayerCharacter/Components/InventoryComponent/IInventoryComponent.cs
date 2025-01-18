using Arenar.Items;

namespace Arenar.Character
{
    public interface IInventoryComponent : ICharacterComponent
    {
        public FirearmWeapon CurrentActiveWeapon { get; }
        
        public FirearmWeapon[] EquippedFirearmWeapons { get; }
        
        
        void ChangeActiveWeapon(int index);

        void AddEquippedFirearmWeapon(ItemInventoryData itemInventoryData, int orderIndex);
    }
}