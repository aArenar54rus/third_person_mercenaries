namespace Arenar.Character
{
    public interface ICharacterSkinComponent : ICharacterComponent
    {
        void SetSkin();

        void SetWeapon(ItemData itemData);
    }
}