using Arenar.Services;
using Zenject;

namespace Arenar.Installers
{
    public class CurrencyServiceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ICurrencyService>().To<CurrencyService>().AsSingle().NonLazy();
        }
    }
}