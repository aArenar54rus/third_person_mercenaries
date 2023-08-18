using Arenar.Character;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;


namespace Arenar
{
    public class CharacterFactory : ICharacterEntityFactory<ComponentCharacterController>
    {
        private readonly DiContainer container;
        private TickableManager tickableManager;
        private InitializableManager initializableManager;


        public CharacterFactory(DiContainer container, TickableManager tickableManager, InitializableManager initializableManager)
        {
            this.container = container;
            this.tickableManager = tickableManager;
            this.initializableManager = initializableManager;
        }

        
        public ComponentCharacterController Create(ComponentCharacterController prototypePrefab, Transform parent)
        {
            var subContainer = container.CreateSubContainer();
            ComponentCharacterController componentCharacterControl = Object
                .Instantiate(prototypePrefab, parent)
                .GetComponent<ComponentCharacterController>();
            
            InstallPostBindings(subContainer, componentCharacterControl);
            subContainer.Inject(componentCharacterControl);

            return componentCharacterControl;
        }

        private void InstallPostBindings(DiContainer subContainer,
            ComponentCharacterController componentCharacterController)
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
                .To<ComponentCharacterController>()
                .FromInstance(componentCharacterController)
                .AsSingle();

            subContainer.Install<PlayerCharacterComponentsInstaller>();
        }
    }
}