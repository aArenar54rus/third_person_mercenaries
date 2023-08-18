using Arenar;
using Arenar.Character;
using Zenject;


public class PlayerSpawnerInstaller : MonoInstaller<PlayerSpawnerInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<ICharacterEntityFactory<ComponentCharacterController>>()
            .To<CharacterFactory>()
            .AsSingle();

        Container.Bind<PlayerCharacterSpawnController>()
            .AsSingle();
    }
}
