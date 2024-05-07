using Arenar.Services.Localization;
using Zenject;


public class LocalizationServiceInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        # if YG_PLUGIN_YANDEX_GAME
        Container.Bind<ILocalizationService>()
            .To<I2LocalizationYandexService>()
            .AsSingle()
            .NonLazy();
        #else
        Container.Bind<ILocalizationService>()
            .To<I2LocalizationService>()
            .AsSingle()
            .NonLazy();
        #endif
    }
}