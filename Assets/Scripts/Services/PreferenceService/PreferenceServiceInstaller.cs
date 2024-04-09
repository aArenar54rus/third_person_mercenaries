using TakeTop.PreferenceSystem;
using Zenject;


namespace Arenar.Services.PreferenceService
{
    public class PreferenceServiceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            /*Container.Bind<IPreferenceManager>()
                .To<PreferenceManager>()
                .AsCached();*/
            //.AsSingle()
            //.NonLazy();
        }
    }
}