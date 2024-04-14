using System.Collections.Generic;
using UnityEngine;


namespace Arenar.SimpleYandexGames
{
    public class YandexGamesPlayerData
    {
        public string uniqueId;
        public string playerName;
        public Dictionary<PlayerIconSize, Texture2D> icons;


        public YandexGamesPlayerData()
        {
            uniqueId = "";
            playerName = "";

            icons = new Dictionary<PlayerIconSize, Texture2D>();
            icons.Add(PlayerIconSize.SmallIcon, null);
            icons.Add(PlayerIconSize.MediumIcon, null);
            icons.Add(PlayerIconSize.LargeIcon, null);
        }
    }
}