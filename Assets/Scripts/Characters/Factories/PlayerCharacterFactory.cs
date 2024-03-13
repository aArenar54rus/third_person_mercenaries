using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;


namespace Arenar.Character
{
    public class PlayerCharacterFactory : ICharacterEntityFactory<ComponentCharacterController>
    {
        private string _playerCharacterAddrPath;
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
            _playerCharacterAddrPath = addressablesCharacters.AddressablesPlayerPath;
        }
        
        
        public ComponentCharacterController Create(Transform parent)
        {
            ComponentCharacterController playerCharacterController = null;
            DiContainer subContainer = container.CreateSubContainer();

            var handle = Addressables.LoadAssetAsync<ComponentCharacterController>(_playerCharacterAddrPath);
            handle.WaitForCompletion();
            playerCharacterController = handle.Result;

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