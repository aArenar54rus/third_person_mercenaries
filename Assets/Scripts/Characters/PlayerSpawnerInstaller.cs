using Arenar;
using Arenar.Character;
using Zenject;


public class PlayerSpawnerInstaller : MonoInstaller<PlayerSpawnerInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<ICharacterEntityFactory<ComponentCharacterController>>()
            .To<CharacterFactory>()
            .AsSingle().NonLazy();

        Container.Bind<TestCharacterSpawnController>()
            .AsSingle().NonLazy();
    }
}
