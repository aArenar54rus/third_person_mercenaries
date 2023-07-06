using Arenar;
using Arenar.Character;
using Zenject;


public class PlayerSpawnerInstaller : MonoInstaller<PlayerSpawnerInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<ICharacterEntityFactory<PlayerCharacterController>>()
            .To<CharacterFactory>()
            .AsSingle();

        Container.Bind<PlayerCharacterSpawnController>()
            .AsSingle();
    }
}
