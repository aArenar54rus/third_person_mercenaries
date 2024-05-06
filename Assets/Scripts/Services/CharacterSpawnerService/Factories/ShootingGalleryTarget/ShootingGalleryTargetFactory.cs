using Arenar.Services;
using Arenar.Services.LevelsService;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;


namespace Arenar.Character
{
    public class ShootingGalleryTargetFactory : ICharacterEntityFactory<ShootingGalleryTargetCharacterController>
    {
        private readonly string _characterResourcePath;
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
            _characterResourcePath = addressablesCharacters.NpcCharacterDatas[NpcType.ShootingGalleryTarget].CharacterPrefabResources;
        }
        

        public ShootingGalleryTargetCharacterController Create(Transform parent)
        {
            ShootingGalleryTargetCharacterController characterController = null;
            
            var subContainer = _container.CreateSubContainer();
            InstallPreBindings(subContainer);

                        
            var prefab = Resources.Load<GameObject>(_characterResourcePath);
            prefab = GameObject.Instantiate(prefab, parent);
            characterController = prefab.GetComponent<ShootingGalleryTargetCharacterController>();

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