using UnityEngine;
using Zenject;


public class ItemSpawnerInstaller : MonoInstaller<ItemSpawnerInstaller>
{
    [SerializeField] private ItemCollectionData itemCollectionData;
    
    
    public override void InstallBindings()
    {
        Container.Bind<IItemFactory<ItemWorldObjectControl>>()
            .To<WorldItemFactory>()
            .AsSingle();

        Container.Bind<ItemCollectionData>()
            .FromInstance(itemCollectionData)
            .AsSingle();
    }
}
