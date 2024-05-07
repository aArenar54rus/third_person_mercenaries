using Arenar.Options;
using DG.Tweening;
using UnityEngine;
using YG;

namespace Arenar.Services.Localization
{
    public class I2LocalizationYandexService : I2LocalizationService
    {
        private Tween _loadingYandexLangTween;


        public I2LocalizationYandexService(IOptionsController optionsController) : base(optionsController)
        {
            _optionsController = optionsController;
            _languageOptions = _optionsController.GetOption<LanguageOptions>();
            Initialize();

        }

        public override void Initialize()
        {
            base.Initialize();
            _loadingYandexLangTween =
                DOVirtual.DelayedCall(0.1f, YandexLoadingLanguage);
        }
        
        private void YandexLoadingLanguage()
        {
            CurrentLanguage = YandexGame.lang;
            YandexGame.SwitchLangEvent += (languageName) =>
            {
                CurrentLanguage = languageName;
            };
        }
    }
}