using Zenject;

namespace TakeTop.Options
{
    public class OptionsSystemServiceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IOptionsController>()
                .To<OptionsController>()
                .AsSingle()
                .NonLazy();
        }
    }
}
