using UnityEngine;
using Zenject;

namespace Arenar.Character
{
    public class PhysicsHumanoidCharacterFactory : ICharacterEntityFactory<PhysicalHumanoidComponentCharacterController>
    {
        private readonly Transform characterRoot;

        
        private readonly SerializableDictionary<CharacterTypeKeys, AddressablesCharacters.CharacterData> characterResourcePaths;
        private readonly DiContainer container;
        private readonly TickableManager tickableManager;
        private readonly InitializableManager initializableManager;


        public PhysicsHumanoidCharacterFactory(DiContainer container,
                                               TickableManager tickableManager,
                                               InitializableManager initializableManager,
                                               AddressablesCharacters addressablesCharacters)
        {
            this.container = container;
            this.tickableManager = tickableManager;
            this.initializableManager = initializableManager;
            characterResourcePaths = addressablesCharacters.ResourcesPhysicsHumanoidPrefab;
            
            characterRoot = new GameObject("PhysicsHumanoidCharacterRoot").transform;
        }
        
        
        public PhysicalHumanoidComponentCharacterController Create(CharacterTypeKeys characterEntityType)
        {
            PhysicalHumanoidComponentCharacterController playerCharacterController = null;
            DiContainer subContainer = container.CreateSubContainer();
            
            var prefab = Resources.Load<GameObject>(characterResourcePaths[characterEntityType].CharacterPrefabResources);
            prefab = GameObject.Instantiate(prefab, characterRoot);
            playerCharacterController = prefab.GetComponent<PhysicalHumanoidComponentCharacterController>();

            InstallPostBindings(subContainer, playerCharacterController, characterEntityType);
            subContainer.Inject(playerCharacterController);
            return playerCharacterController;
        }

        private void InstallPostBindings(DiContainer subContainer,
                                         PhysicalHumanoidComponentCharacterController characterControl,
                                         CharacterTypeKeys characterEntityType)
        {
            subContainer.ResolveRoots();
            
            subContainer.Rebind<TickableManager>()
                .FromInstance(tickableManager)
                .NonLazy();
            
            subContainer.Rebind<InitializableManager>()
                .FromInstance(initializableManager)
                .NonLazy();
            
            subContainer.Bind(typeof(ICharacterEntity),
                    typeof(ICharacterDataStorage<CharacterAudioDataStorage>),
                    typeof(ICharacterDataStorage<CharacterVisualDataStorage>),
                    typeof(ICharacterDataStorage<CharacterAnimatorDataStorage>),
                    typeof(ICharacterDataStorage<CharacterPhysicsDataStorage>),
                    typeof(ICharacterDataStorage<CharacterAimAnimationDataStorage>))
                .To<PhysicalHumanoidComponentCharacterController>()
                .FromInstance(characterControl)
                .AsSingle();

            switch (characterEntityType)
            {
                case CharacterTypeKeys.Player:
                    subContainer.Install<PlayerCharacterComponentsInstaller>();
                    break;
                
                case CharacterTypeKeys.DefaultKnight:
                    subContainer.Install<DefaultKnightComponentsInstaller>();
                    break;
            }
        }
    }
}