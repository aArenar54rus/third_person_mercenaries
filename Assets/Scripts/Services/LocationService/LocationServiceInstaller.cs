using Zenject;


namespace Arenar.LocationService
{
    public class LocationServiceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ILocationService>()
                .To<LocationService>()
                .AsSingle()
                .NonLazy();
        }
    }
}