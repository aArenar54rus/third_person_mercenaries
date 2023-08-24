using Arenar.Character;
using Zenject;


namespace Arenar.Services.InventoryService
{
    public class EquipItemFactoriesInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<FirearmWeaponFactory>().AsSingle().NonLazy();
        }
    }
}