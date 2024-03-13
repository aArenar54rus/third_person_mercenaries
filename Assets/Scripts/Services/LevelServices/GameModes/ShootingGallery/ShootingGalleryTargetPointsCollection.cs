using UnityEngine;
using Zenject;


namespace Arenar.Services.LevelsService
{
    public class ShootingGalleryTargetPointsCollection : SubSceneMonoInstaller
    {
        [SerializeField] private SerializableDictionary<int, ShootingGalleryTargetPoint> _targetPoints;


        public override void InstallBindingsIntoContainer(DiContainer container)
        {
            container.Rebind<SerializableDictionary<int, ShootingGalleryTargetPoint>>()
                .FromInstance(_targetPoints)
                .AsSingle()
                .NonLazy();
        }
    }
}