using UnityEngine;

namespace Arenar.Character
{
    public interface ICharacterEntityFactory<TProduct>
    {
        TProduct Create(Transform parent);
    }
}