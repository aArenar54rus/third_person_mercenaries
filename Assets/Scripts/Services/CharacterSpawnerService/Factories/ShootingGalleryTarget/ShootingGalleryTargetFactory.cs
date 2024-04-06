using Arenar.Services;
using Arenar.Services.LevelsService;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;


namespace Arenar.Character
{
    public class ShootingGalleryTargetFactory : ICharacterEntityFactory<ShootingGalleryTargetCharacterController>
    {
        private readonly AddressablesCharacters.CharacterData _characterData;
        private readonly DiContainer _container;
        
        private TickableManager _tickableManager;
        private InitializableManager _initializableManager;


        public ShootingGalleryTargetFactory(DiContainer container,
                                            TickableManager tickableManager,
                                            InitializableManager initializableManager,
                                            AddressablesCharacters addressablesCharacters)
        {
            _container = container;
            _tickableManager = tickableManager;
            _initializableManager = initializableManager;
            _characterData = addressablesCharacters.NpcCharacterDatas[NpcType.ShootingGalleryTarget];
        }
        

        public ShootingGalleryTargetCharacterController Create(Transform parent)
        {
            ShootingGalleryTargetCharacterController characterController = null;
            
            var subContainer = _container.CreateSubContainer();
            InstallPreBindings(subContainer);

            var handle = _characterData.AddressablesCharacter.InstantiateAsync(parent);
            handle.WaitForCompletion();
            characterController = handle.Result.GetComponent<ShootingGalleryTargetCharacterController>();

            InstallPostBindings(subContainer, characterController);
            subContainer.Inject(characterController);

            return characterController;
        }
        
        private void InstallPreBindings(DiContainer subContainer)
        {
            // HACK: for getting inject from loaded battle scene
            foreach (var monoInstaller in Object.FindObjectsOfType<SubSceneMonoInstaller>())
            {
                monoInstaller.InstallBindingsIntoContainer(subContainer);
            }
        }

        private void InstallPostBindings(DiContainer subContainer, ShootingGalleryTargetCharacterController characterControl)
        {
            subContainer.ResolveRoots();

            subContainer.Rebind<TickableManager>()
                .FromInstance(_tickableManager)
                .NonLazy();

            subContainer.Rebind<InitializableManager>()
                .FromInstance(_initializableManager)
                .NonLazy();

            subContainer.Bind(typeof(ICharacterEntity),
                    typeof(ICharacterDataStorage<CharacterAudioDataStorage>),
                    typeof(ICharacterDataStorage<SGTargetPhysicalDataStorage>),
                    typeof(ICharacterDataStorage<SGTargetWeaponDataStorage>))
                .To<ShootingGalleryTargetCharacterController>()
                .FromInstance(characterControl)
                .AsSingle();
            
            subContainer.Install<ShootingGalleryTargetCharacterComponentsInstaller>();
        }
    }
}