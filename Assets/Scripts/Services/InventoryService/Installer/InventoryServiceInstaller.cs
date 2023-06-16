using Zenject;


namespace Arenar.Services.InventoryService
{
    public class InventoryServiceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IInventoryService>()
                .To<InventoryService>()
                .AsSingle()
                .NonLazy();
        }
    }
}