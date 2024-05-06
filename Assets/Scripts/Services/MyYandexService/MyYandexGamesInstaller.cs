using UnityEngine;
using Zenject;

namespace TakeTop.Web
{
    public class MyYandexGamesInstaller : MonoInstaller
    {
        //[SerializeField] private YandexGames _yandexGames;
        
        
        public override void InstallBindings()
        {
            //Container.BindInstance<YandexGames>(_yandexGames)
               // .AsSingle().NonLazy();
        }
    }
}