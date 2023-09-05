using Zenject;


namespace Arenar.Services.PlayerInputService
{
    public class PlayerInputServiceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IPlayerInputService>()
                .To<PlayerInputService>()
                .AsSingle()
                .NonLazy();
        }
    }
}