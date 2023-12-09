using Newtonsoft.Json;

namespace Arenar.Options
{
    public class VibrationOption : IOption
    {
        [JsonProperty] public bool isActive;


        public VibrationOption() =>
            isActive = true;


        [JsonIgnore]
        public bool IsActive
        {
            get => isActive;
            set => isActive = value;
        }
    }
}
