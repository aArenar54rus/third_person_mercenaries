using Arenar.Character;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;


namespace Arenar
{
    public class CharacterFactory : ICharacterEntityFactory<PlayerCharacterController>
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

        
        public PlayerCharacterController Create(PlayerCharacterController prototypePrefab, Transform parent)
        {
            var subContainer = container.CreateSubContainer();
            PlayerCharacterController playerCharacterControl = Object
                .Instantiate(prototypePrefab, parent)
                .GetComponent<PlayerCharacterController>();
            
            InstallPostBindings(subContainer, playerCharacterControl);
            subContainer.Inject(playerCharacterControl);

            return playerCharacterControl;
        }

        private void InstallPostBindings(DiContainer subContainer,
            PlayerCharacterController playerCharacterController)
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
                .To<PlayerCharacterController>()
                .FromInstance(playerCharacterController)
                .AsSingle();

            subContainer.Install<PlayerCharacterComponentsInstaller>();
        }
    }
}