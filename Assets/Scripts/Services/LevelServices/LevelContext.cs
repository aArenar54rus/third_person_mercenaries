using System.Collections.Generic;

namespace Arenar.Services.LevelsService
{
    public class LevelContext
    {
        public LevelData LevelData { get; }
        
        public LevelDifficult LevelDifficult { get; }

        public GameMode GameMode { get; }

        public Dictionary<LevelMarkType, bool> LevelMarkTypeSuccess { get; set; } = new Dictionary<LevelMarkType, bool>()
        {
            { LevelMarkType.ClearLevel, false },
            { LevelMarkType.Genocide, false },
            { LevelMarkType.NoDamage, false },
            { LevelMarkType.TimeSuccess, false },
        };

        public int LevelScore { get; set; } = 0;
        
        
        public LevelContext(LevelData levelData, LevelDifficult levelDifficult, GameMode gameMode)
        {
            LevelData = levelData;
            LevelDifficult = levelDifficult;

            GameMode = gameMode;
            LevelScore = 0;
        }
    }
}