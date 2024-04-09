using System;
using Newtonsoft.Json;

namespace Arenar.Services.SaveAndLoad
{
    [Serializable]
    public class PlayerSaveDelegate 
    {
        [JsonProperty] public string playerName = "Player";
        [JsonProperty] public int playerCharacterLevel = 0;
        [JsonProperty] public int currentXpPoints = 0;


        public PlayerSaveDelegate()
        {
            playerCharacterLevel = 0;
            currentXpPoints = 0;
        }
    }
}