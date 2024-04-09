using System.Collections.Generic;

namespace Arenar.Services.LevelsService
{
    public class LevelContext
    {
        private const int MARK_SCORE_COST = 100;
        private const int TARGET_SCORE_COST = 10;


        public enum GameResult
        {
            None = 0,
            Victory = 1,
            Defeat = 2,
        }
        
        
        public LevelData LevelData { get; }
        
        public LevelDifficult LevelDifficult { get; }

        public GameMode GameMode { get; }

        public Dictionary<LevelMarkType, bool> LevelMarkTypeResult { get; private set; }
        
        public GameResult GameResultStatus { get; private set; }
        
        public int NeededTargetCount { get; set; }
        
        public int CurrentTargetCount { get; set; }
        
        public int SettedDamage { get; set; }
        
        public int GettedDamage { get; set; }
        
        public int PlayerDeath { get; set; }

        public int Score { get; set; } = 0;
        
        
        public LevelContext(LevelData levelData, LevelDifficult levelDifficult, GameMode gameMode, int neededTargetCount)
        {
            LevelData = levelData;
            LevelDifficult = levelDifficult;

            GameMode = gameMode;
            Score = 0;

            NeededTargetCount = neededTargetCount;
            CurrentTargetCount = 0;
            SettedDamage = 0;
            GettedDamage = 0;
            PlayerDeath = 0;
            GameResultStatus = GameResult.None;
            
            LevelMarkTypeResult = new Dictionary<LevelMarkType, bool>()
            {
                { LevelMarkType.ClearLevel, false },
                { LevelMarkType.Genocide, false },
                { LevelMarkType.NoDamage, false },
                { LevelMarkType.TimeSuccess, false },
            };
        }
        
        
        public void CompleteLevel(GameResult result)
        {
            GameResultStatus = result;
            
            LevelMarkTypeResult[LevelMarkType.ClearLevel] = (result == GameResult.Victory);
            LevelMarkTypeResult[LevelMarkType.NoDamage] = (GettedDamage == 0);
            LevelMarkTypeResult[LevelMarkType.Genocide] = (NeededTargetCount == CurrentTargetCount);
            LevelMarkTypeResult[LevelMarkType.TimeSuccess] = false;

            UpdateScore();
        }
        
        private void UpdateScore()
        {
            foreach (var mark in LevelMarkTypeResult)
            {
                if (mark.Value)
                    Score += MARK_SCORE_COST;
            }

            Score += CurrentTargetCount * TARGET_SCORE_COST;
        }
    }
}