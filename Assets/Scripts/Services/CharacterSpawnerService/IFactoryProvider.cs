using UnityEngine;


namespace Arenar.SpawnerFactory
{
    public interface IFactoryProvider<TProduct>
        where TProduct : ISpawnerElement
    {
        TProduct Create(TProduct prototypePrefab, Transform parent);
    }
}