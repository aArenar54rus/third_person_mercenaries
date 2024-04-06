using UnityEngine;
using Zenject;


namespace Arenar
{
    public class ItemProjectileInstaller : MonoInstaller
    {
        [SerializeField] private ItemProjectileDataSO data;
        
        
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<EffectsSpawner>()
                .AsSingle()
                .NonLazy();
            
            Container.BindInstance(data)
                .AsSingle()
                .NonLazy();
        }
    }
}