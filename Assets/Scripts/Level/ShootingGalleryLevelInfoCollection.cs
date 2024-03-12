using System;
using UnityEngine;
using Zenject;


namespace Arenar.Services.LevelsService
{
    [CreateAssetMenu(menuName = "LevelsData/ShootingGalleryLevelInfoCollection")]
    public class ShootingGalleryLevelInfoCollection : ScriptableObjectInstaller<ShootingGalleryLevelInfoCollection>
    {
        [SerializeField] private SerializableDictionary<int, ShootingGalleryTargetNode[]> _shootingGalleriesInfos;
        
        
        public override void InstallBindings()
        {
            Container.BindInstance(_shootingGalleriesInfos).AsSingle();
        }
    }
}