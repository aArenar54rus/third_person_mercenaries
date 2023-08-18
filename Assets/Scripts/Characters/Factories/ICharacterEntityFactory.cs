using UnityEngine;

namespace Arenar.Character
{
    public interface ICharacterEntityFactory<TProduct>
    {
        TProduct Create(TProduct prototypePrefab, Transform parent);
    }
}