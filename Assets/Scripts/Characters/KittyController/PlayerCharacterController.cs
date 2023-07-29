using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace Arenar.Character
{
    public class PlayerCharacterController : MonoBehaviour,
                                   ICharacterEntity,
                                   ICharacterDataStorage<CharacterAudioDataStorage>,
                                   ICharacterDataStorage<CharacterVisualDataStorage>,
                                   ICharacterDataStorage<CharacterAnimatorDataStorage>,
                                   ICharacterDataStorage<CharacterPhysicsDataStorage>,
                                   ICharacterDataStorage<CharacterAimAnimationDataStorage>
    {
        private Dictionary<Type, ICharacterComponent> characterComponentsPool;
        
        [SerializeField] private CharacterAudioDataStorage characterAudioDataStorage;
        [SerializeField] private CharacterVisualDataStorage characterVisualDataStorage;
        [SerializeField] private CharacterAnimatorDataStorage characterAnimatorDataStorage;
        [SerializeField] private CharacterPhysicsDataStorage characterPhysicsDataStorage;
        [SerializeField] private CharacterAimAnimationDataStorage characterAimAnimationDataStorage;


        public Transform CharacterTransform => characterPhysicsDataStorage.CharacterTransform;
        
        public Transform CameraTransform => characterPhysicsDataStorage.CameraTransform;
        
        CharacterAudioDataStorage ICharacterDataStorage<CharacterAudioDataStorage>.Data => characterAudioDataStorage;

        CharacterVisualDataStorage ICharacterDataStorage<CharacterVisualDataStorage>.Data => characterVisualDataStorage;

        CharacterAnimatorDataStorage ICharacterDataStorage<CharacterAnimatorDataStorage>.Data => characterAnimatorDataStorage;
        
        CharacterPhysicsDataStorage ICharacterDataStorage<CharacterPhysicsDataStorage>.Data => characterPhysicsDataStorage;
        
        CharacterAimAnimationDataStorage ICharacterDataStorage<CharacterAimAnimationDataStorage>.Data => characterAimAnimationDataStorage;


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
        }
        
        public TCharacterComponent TryGetCharacterComponent<TCharacterComponent>(out bool isSuccess)
            where TCharacterComponent : ICharacterComponent
        {
            if (!characterComponentsPool.ContainsKey(typeof(TCharacterComponent)))
            {
                Debug.LogError($"Not found component {typeof(TCharacterComponent)}");
                isSuccess = false;
                return default;
            }
            
            isSuccess = true;
            return (TCharacterComponent) characterComponentsPool[typeof(TCharacterComponent)];
        }
        
        private void Initialize()
        {
            foreach (var characterComponent in characterComponentsPool)
                characterComponent.Value.Initialize();
        }

        private void DeInitialize()
        {
            foreach (var characterComponent in characterComponentsPool)
                characterComponent.Value.DeInitialize();
        }

        private void OnDestroy()
        {
            DeInitialize();
        }
    }
}