using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace Arenar.Services.LevelsService
{
    public class ShootingGalleryTargetPointsCollection : SubSceneMonoInstaller
    {
        [SerializeField] private List<ShootingGalleryTargetPoint> _targetPoints;


        public override void InstallBindingsIntoContainer(DiContainer container)
        {
            container.Rebind<List<ShootingGalleryTargetPoint>>()
                .FromInstance(_targetPoints)
                .AsSingle()
                .NonLazy();
        }
    }
}