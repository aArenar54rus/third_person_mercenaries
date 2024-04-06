using Arenar.Services.LevelsService;
using UnityEngine;
using Zenject;


namespace Arenar.Services.InventoryService
{
    public class LevelsServiceInstaller : MonoInstaller
    {
        [SerializeField] private ShootingGalleryLevelInfoCollection _shootingGalleryLevelInfoCollection;
        
        
        public override void InstallBindings()
        {
            Container.Bind<ILevelsService>()
                     .To<LevelsService.LevelsService>()
                     .AsSingle()
                     .NonLazy();
            
            Container.BindInstance(_shootingGalleryLevelInfoCollection)
                .AsSingle()
                .NonLazy();
        }
    }
}