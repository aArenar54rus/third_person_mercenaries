using Arenar.Items;

namespace Arenar.Character
{
    public interface IInventoryComponent : ICharacterComponent
    {
        public MeleeWeapon CurrentActiveMeleeWeapon { get; }
        public FirearmWeapon CurrentActiveFirearmWeapon { get; }
        
        public FirearmWeapon[] EquippedFirearmWeapons { get; }
        
        
        void ChangeActiveWeapon(int index);

        void AddEquippedFirearmWeapon(ItemInventoryData itemInventoryData, int orderIndex);
    }
}