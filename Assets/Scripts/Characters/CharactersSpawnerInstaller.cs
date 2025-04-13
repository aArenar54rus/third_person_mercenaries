using Arenar.Character;
using UnityEngine;
using Zenject;


namespace Arenar.Installers
{
    public class CharactersSpawnerInstaller : MonoInstaller<CharactersSpawnerInstaller>
    {
        [SerializeField] private AddressablesCharacters _addressablesCharacters;
        [SerializeField] private PlayerCharacterLevelData _playerCharacterLevelData;

        
        public override void InstallBindings()
        {
            Container.Bind<ICharacterEntityFactory<PhysicalHumanoidComponentCharacterController>>()
                .To<PhysicsHumanoidCharacterFactory>()
                .AsSingle().NonLazy();

            Container.Bind<ICharacterEntityFactory<ShootingGalleryTargetCharacterController>>()
                .To<ShootingGalleryTargetFactory>()
                .AsSingle().NonLazy();

            Container.BindInstance(_addressablesCharacters)
                .AsSingle().NonLazy();

            Container.BindInstance(_playerCharacterLevelData)
                .AsSingle().NonLazy();

            Container.Bind<CharacterSpawnController>()
                .AsSingle().NonLazy();
        }
    }
}
