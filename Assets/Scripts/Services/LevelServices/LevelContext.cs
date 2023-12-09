using System.Collections.Generic;

namespace Arenar.Services.LevelsService
{
    public class LevelContext
    {
        public LevelData LevelData { get; }
        
        public LevelDifficult LevelDifficult { get; }

        public LevelType LevelType { get; }

        public Dictionary<LevelMarkType, bool> LevelMarkTypeSuccess { get; set; } = new Dictionary<LevelMarkType, bool>()
        {
            { LevelMarkType.ClearLevel, false },
            { LevelMarkType.Genocide, false },
            { LevelMarkType.NoDamage, false },
            { LevelMarkType.TimeSuccess, false },
        };

        public int LevelScore { get; set; } = 0;
        
        
        public LevelContext(LevelData levelData, LevelDifficult levelDifficult, LevelType levelType = LevelType.Campaing)
        {
            LevelData = levelData;
            LevelDifficult = levelDifficult;

            LevelType = levelType;
            LevelScore = 0;
        }
    }
}