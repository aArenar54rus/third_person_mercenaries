using Newtonsoft.Json;

namespace TakeTop.Options
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
