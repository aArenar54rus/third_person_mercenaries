using UnityEngine;


namespace Arenar
{
    public interface ICharacterEntityFactory<TCharacter>
    {
        TCharacter Create(TCharacter prototypePrefab, Transform parent);
    }
}