using UnityEngine;
using Zenject;


namespace Arenar.Character
{
    public class CharacterFactory : ICharacterEntityFactory<ComponentCharacterController>
    {
        private readonly DiContainer container;
        private readonly TickableManager tickableManager;
        private readonly InitializableManager initializableManager;


        public CharacterFactory(DiContainer container, TickableManager tickableManager, InitializableManager initializableManager)
        {
            this.container = container;
            this.tickableManager = tickableManager;
            this.initializableManager = initializableManager;
        }
        
        
        public ComponentCharacterController Create(
            ComponentCharacterController prototypePrefab,
            Transform parent)
        {
            DiContainer subContainer = container.CreateSubContainer();
            
            ComponentCharacterController characterController = Object.Instantiate(prototypePrefab.gameObject, parent)
                .GetComponent<ComponentCharacterController>();

            InstallPostBindings(subContainer, characterController);
            subContainer.Inject(characterController);
            return characterController;
        }

        private void InstallPostBindings(DiContainer subContainer, ComponentCharacterController characterControl)
        {
            subContainer.ResolveRoots();
            
            subContainer.Rebind<TickableManager>()
                .FromInstance(tickableManager)
                .NonLazy();
            
            subContainer.Rebind<InitializableManager>()
                .FromInstance(initializableManager)
                .NonLazy();

            switch (characterControl)
            {
                case PuppetComponentCharacterController:
                    subContainer.Install<PuppetCharacterComponentsInstaller>();
                    break;
                    
                case PlayerComponentCharacterController:
                    subContainer.Bind(typeof(ICharacterEntity),
                            typeof(ICharacterDataStorage<CharacterAudioDataStorage>),
                            typeof(ICharacterDataStorage<CharacterVisualDataStorage>),
                            typeof(ICharacterDataStorage<CharacterAnimatorDataStorage>),
                            typeof(ICharacterDataStorage<CharacterPhysicsDataStorage>),
                            typeof(ICharacterDataStorage<CharacterAimAnimationDataStorage>))
                        .To<PlayerComponentCharacterController>()
                        .FromInstance(characterControl)
                        .AsSingle();
                    subContainer.Install<PlayerCharacterComponentsInstaller>();
                    break;
            }
        }
    }
}