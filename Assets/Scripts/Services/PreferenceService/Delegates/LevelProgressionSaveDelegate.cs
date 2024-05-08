using System;
using Newtonsoft.Json;


namespace Arenar.Services.SaveAndLoad
{
    [Serializable]
    public class LevelProgressionSaveDelegate 
    {
        [JsonProperty] public int completedLevel;
        [JsonProperty] public LevelDifficult completeDifficult;


        public LevelProgressionSaveDelegate()
        {
            completedLevel = 0;
            completeDifficult = LevelDifficult.None;
        }
    }
}