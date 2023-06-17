using UnityEngine;


namespace Arenar.Character
{
    public interface ICharacterEntity
    {
        Transform CharacterTransform { get; }


        TCharacterComponent TryGetCharacterComponent<TCharacterComponent>(out bool isSuccess)
            where TCharacterComponent : ICharacterComponent;

        void ReInitialize();
    }
}