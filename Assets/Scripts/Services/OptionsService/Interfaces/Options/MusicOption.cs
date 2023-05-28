using System;
using Newtonsoft.Json;
using UnityEngine;

namespace TakeTop.Options
{
    [Serializable]
    public class MusicOption : IOption
    {
        private const float MAX_VOLUME = 1f;
        
        
        [JsonProperty] private bool isActive;
        [JsonProperty] private float volume;


        public MusicOption()
        {
            isActive = true;
            volume = MAX_VOLUME;
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
