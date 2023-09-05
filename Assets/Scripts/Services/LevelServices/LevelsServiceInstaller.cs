using Arenar.Services.LevelsService;
using Zenject;


namespace Arenar.Services.InventoryService
{
    public class LevelsServiceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ILevelsService>()
                     .To<LevelsService.LevelsService>()
                     .AsSingle()
                     .NonLazy();
        }
    }
}