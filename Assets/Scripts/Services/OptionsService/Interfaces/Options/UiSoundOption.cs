using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Arenar.Options
{
    [Serializable]
    public class UiSoundOption : IOption
    {
        private const float MAX_VOLUME = 1f;
        
        
        [JsonProperty] public bool isActive;
        [JsonProperty] public float volume;


        public UiSoundOption()
        {
            this.isActive = true;
            this.volume = MAX_VOLUME;
        }
        

        [JsonIgnore]
        public bool IsActive
        {
            get => isActive;
            set => isActive = value;
        }
        
        [JsonIgnore]
        public float Volume
        {
            get => volume;
            set => volume = Mathf.Clamp01(value);
        }
    }
}
