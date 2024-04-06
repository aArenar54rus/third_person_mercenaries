using Arenar.Character;
using UnityEngine;
using Zenject;


namespace Arenar.Installers
{
    public class CharactersSpawnerInstaller : MonoInstaller<CharactersSpawnerInstaller>
    {
        [SerializeField] private AddressablesCharacters _addressablesCharacters;
        [SerializeField] private ShootingGalleryTargetParameters _shootingGalleryTargetParameters;

        
        public override void InstallBindings()
        {
            Container.Bind<ICharacterEntityFactory<ComponentCharacterController>>()
                .To<PlayerCharacterFactory>()
                .AsSingle().NonLazy();

            Container.Bind<ICharacterEntityFactory<ShootingGalleryTargetCharacterController>>()
                .To<ShootingGalleryTargetFactory>()
                .AsSingle().NonLazy();

            Container.BindInstance(_addressablesCharacters)
                .AsSingle().NonLazy();

            BindNpcBaseParameters();

            Container.Bind<CharacterSpawnController>()
                .AsSingle().NonLazy();
        }
        
        private void BindNpcBaseParameters()
        {
            Container.BindInstance(_shootingGalleryTargetParameters)
                .AsSingle().NonLazy();
        }
    }
}
