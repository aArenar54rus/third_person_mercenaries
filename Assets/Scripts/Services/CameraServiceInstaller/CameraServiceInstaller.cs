using Arenar.CameraService;
using Cinemachine;
using UnityEngine;
using Zenject;


namespace Arenar.Services
{
    public class CameraServiceInstaller : MonoInstaller
    {
        [SerializeField] private Camera mainCamera = default;
        [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

        
        public override void InstallBindings()
        {
            Container.Bind<Camera>()
                .FromInstance(mainCamera)
                .AsSingle()
                .NonLazy();
            
            Container.Bind<CinemachineVirtualCamera>()
                .FromInstance(cinemachineVirtualCamera)
                .AsSingle()
                .NonLazy();

            Container.Bind<ICameraService>()
                .To< Arenar.CameraService.CameraService>()
                .AsSingle()
                .NonLazy();
            
            Container.Bind<CameraStatesFactory>()
                .AsSingle()
                .NonLazy();
        }
    }
}