using Arenar;
using UnityEngine;
using Zenject;

namespace TakeTop.Web
{
    public class MyYandexGamesInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            YandexGamesAdsService service =
               new GameObject("Yandex Games Ads Service")
                   .AddComponent<YandexGamesAdsService>();
            
            Container.Bind<YandexGamesAdsService>()
                .FromInstance(service)
                .AsSingle()
                .NonLazy();
        }
    }
}