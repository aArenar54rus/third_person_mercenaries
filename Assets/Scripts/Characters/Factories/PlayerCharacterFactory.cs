using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;


namespace Arenar.Character
{
    public class PlayerCharacterFactory : ICharacterEntityFactory<ComponentCharacterController>
    {
        private readonly AssetReference _playerCharacterAddrPath;
        private readonly DiContainer container;
        private readonly TickableManager tickableManager;
        private readonly InitializableManager initializableManager;


        public PlayerCharacterFactory(DiContainer container,
            TickableManager tickableManager,
            InitializableManager initializableManager,
            AddressablesCharacters addressablesCharacters)
        {
            this.container = container;
            this.tickableManager = tickableManager;
            this.initializableManager = initializableManager;
            _playerCharacterAddrPath = addressablesCharacters.AddressablesPlayer;
        }
        
        
        public ComponentCharacterController Create(Transform parent)
        {
            ComponentCharacterController playerCharacterController = null;
            DiContainer subContainer = container.CreateSubContainer();

            var handle = _playerCharacterAddrPath.InstantiateAsync(parent);
            handle.WaitForCompletion();
            playerCharacterController = handle.Result.GetComponent<ComponentCharacterController>();

            InstallPostBindings(subContainer, playerCharacterController);
            subContainer.Inject(playerCharacterController);
            return playerCharacterController;
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
        }
    }
}