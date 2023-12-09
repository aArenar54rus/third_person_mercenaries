using Arenar.Options;
using DG.Tweening;
using I2.Loc;
//using Arenar.SimpleYandexGames;

namespace Arenar.Services.Localization
{
    public class I2LocalizationYandexService : I2LocalizationService
    {
        private Tween _loadingYandexLangTween;

        //[Inject] private YandexGames yandexGames;

        
        public I2LocalizationYandexService(IOptionsController optionsController) : base(optionsController)
        {
            _optionsController = optionsController;
            _languageOptions = _optionsController.GetOption<LanguageOptions>();
            Initialize();

        }

        public override void Initialize()
        {
            _languages = LocalizationManager.GetAllLanguages(true).ToArray();
            
            _loadingYandexLangTween =
                DOVirtual.DelayedCall(0.1f, YandexLoadingLanguage)
                    .SetLoops(-1);
        }
        
        private void YandexLoadingLanguage()
        {
            /*while (!yandexGames.IsInitialized)
                yield return null;
            
            var currentLanguageKey = yandexGames.CurrentLanguage;
            SetLanguage(currentLanguageKey); */
        }
    }
}