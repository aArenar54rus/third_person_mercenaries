using Zenject;


namespace Arenar.Services.SaveAndLoad
{
    public class SaveAndLoadServiceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ISaveAndLoadService<SaveDelegate>>()
                .To<SaveAndLoadService>()
                .AsSingle()
                .NonLazy();
        }
    }
}