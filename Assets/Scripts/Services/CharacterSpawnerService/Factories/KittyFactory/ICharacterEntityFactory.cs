using UnityEngine;


namespace CatSimulator
{
    public interface ICharacterEntityFactory<TCharacter>
    {
        TCharacter Create(TCharacter prototypePrefab, Transform parent);
    }
}