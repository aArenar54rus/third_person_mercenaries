namespace Arenar.Character
{
    public interface ICharacterSkinComponent : ICharacterComponent
    {
        void SetSkin();

        void SetWeapon(ItemInventoryData itemInventoryData);
    }
}