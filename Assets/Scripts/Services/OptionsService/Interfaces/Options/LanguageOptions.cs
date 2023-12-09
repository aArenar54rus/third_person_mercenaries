using System;
using Newtonsoft.Json;


namespace Arenar.Options
{
    [Serializable]
    public class LanguageOptions : IOption
    {
        [JsonProperty] private string languageKey;


        public LanguageOptions()
        {
            languageKey = "";
        }
        
        
        [JsonIgnore]
        public string LanguageKey
        {
            get => languageKey;
            set => languageKey = value;
        }
    }
}