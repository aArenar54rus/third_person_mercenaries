using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace Arenar.Character
{
    public abstract class ComponentCharacterController : MonoBehaviour, ICharacterEntity
    {
        private Dictionary<Type, ICharacterComponent> characterComponentsPool;


        public abstract Transform CharacterTransform { get; }


        [Inject]
        public void Construct(Dictionary<Type, ICharacterComponent> characterComponentsPool)
        {
            this.characterComponentsPool = characterComponentsPool;
            Initialize();
        }
        
        public void ReInitialize()
        {
            foreach (var characterComponent in characterComponentsPool)
                characterComponent.Value.OnStart();
            
            gameObject.SetActive(true);
        }
        
        public bool TryGetCharacterComponent<TCharacterComponent>(out TCharacterComponent resultComponent)
            where TCharacterComponent : ICharacterComponent
        {
            if (!characterComponentsPool.ContainsKey(typeof(TCharacterComponent)))
            {
                Debug.LogError($"Not found component {typeof(TCharacterComponent)}");
                resultComponent = default;
                return false;
            }
            
            resultComponent = (TCharacterComponent) characterComponentsPool[typeof(TCharacterComponent)];
            return true;
        }
        
        private void Initialize()
        {
            foreach (var characterComponent in characterComponentsPool)
                characterComponent.Value.Initialize();
        }

        public void DeInitialize()
        {
            foreach (var characterComponent in characterComponentsPool)
                characterComponent.Value.DeInitialize();
        }

        private void OnDestroy() =>
            DeInitialize();
    }
}