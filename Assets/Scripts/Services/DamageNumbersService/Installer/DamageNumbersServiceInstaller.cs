using UnityEngine;
using Zenject;


namespace Arenar.Services.DamageNumbersService
{
    public class DamageNumbersServiceInstaller : MonoInstaller
    {
        [SerializeField] private DamageNumbersPrefabsData _data;


        public override void InstallBindings()
        {
            Container.BindInstance<DamageNumbersPrefabsData>(_data)
                .AsSingle().NonLazy();
            
            Container.Bind<IDamageNumbersService>()
                .To<DamageNumbersService>()
                .AsSingle().NonLazy();
        }
    }
}