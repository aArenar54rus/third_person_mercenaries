namespace Arenar.Services.Localization
{
    public interface ILocalizationService
    {
        public string[] Languages { get; }
        
        public int LanguageIndex { get; }
        
        public string CurrentLanguage { get; set; }


        void Initialize();
    }
}