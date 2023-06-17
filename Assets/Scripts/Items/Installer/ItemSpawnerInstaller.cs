using UnityEngine;
using Zenject;


namespace Arenar.Installers
{
    public class ItemSpawnerInstaller : MonoInstaller<ItemSpawnerInstaller>
    {
        [SerializeField] private ItemCollectionData itemCollectionData;


        public override void InstallBindings()
        {
            Container.Bind<IItemFactory<ItemInteractableElement>>()
                .To<WorldItemFactory>()
                .AsSingle();

            Container.Bind<ItemCollectionData>()
                .FromInstance(itemCollectionData)
                .AsSingle();
        }
    }
}