using UnityEngine;


namespace Arenar.Character
{
    public interface ICharacterEntity
    {
        Transform CharacterTransform { get; }


        bool TryGetCharacterComponent<TCharacterComponent>(out TCharacterComponent resultComponent)
            where TCharacterComponent : ICharacterComponent;

        void Activate();
        
        void DeActivate();

        void Initialize();
        
        void DeInitialize();
    }
}