using Arenar.Services.LevelsService;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Arenar.Services.InventoryService
{
    public class LevelsServiceInstaller : MonoInstaller
    {
        [FormerlySerializedAs("_shootingGalleryLevelInfoCollection"),SerializeField]
        private ClearLocationLevelInfoCollection clearLocationLevelInfoCollection;
        [SerializeField]
        private SurvivalLevelInfoCollection survivalLevelInfoCollection;
        
        
        public override void InstallBindings()
        {
            Container.Bind<ILevelsService>()
                .To<LevelsService.LevelsService>()
                .AsSingle()
                .NonLazy();
            
            Container.BindInstance(clearLocationLevelInfoCollection)
                .AsSingle()
                .NonLazy();
            
            Container.BindInstance(survivalLevelInfoCollection)
                .AsSingle()
                .NonLazy();
        }
    }
}