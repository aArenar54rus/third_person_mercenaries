using Arenar.Options;
using I2.Loc;
using UnityEngine;


namespace Arenar.Services.Localization
{
    public class I2LocalizationService : ILocalizationService
    {
        private const string DEFAULT_LANGUAGE = "en";
        
        
        protected string[] _languages;

        protected IOptionsController _optionsController;
        protected LanguageOptions _languageOptions;


        public string[] Languages => _languages;

        public int LanguageIndex
        {
            get => GetLanguageIndex();
        }

        public string CurrentLanguage
        {
            get => _languageOptions.LanguageKey;
            set
            {
                for (int i = 0; i < _languages.Length; i++)
                {
                    if (_languages[i] != value)
                        continue;

                    _languageOptions.LanguageKey = value;
                    LocalizationManager.CurrentLanguage = value;
                    return;
                }

                Debug.LogError($"Unknown language {value}");
            }
        }


        public I2LocalizationService(IOptionsController optionsController)
        {
            _optionsController = optionsController;
            _languageOptions = _optionsController.GetOption<LanguageOptions>();
            Initialize();
        }
        
        public virtual void Initialize()
        {
            _languages = LocalizationManager.GetAllLanguages(true).ToArray();
            if (_languageOptions.LanguageKey == "")
                _languageOptions.LanguageKey = DEFAULT_LANGUAGE;
        }
        
        private int GetLanguageIndex()
        {
            for (int i = 0; i < _languages.Length; i++)
            {
                if (_languages[i] != _languageOptions.LanguageKey)
                    continue;

                return i;
            }

            return 0;
        }
    }
}