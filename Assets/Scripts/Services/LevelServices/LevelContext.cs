using System.Collections.Generic;
using UnityEngine;

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
        
        public int NeededTargetCount { get; private set; }
        
        public int CurrentTargetCount { get; set; }
        
        public int SettedDamage { get; set; }
        
        public int GettedDamage { get; set; }
        
        public int PlayerDeath { get; set; }

        public int XpPoints { get; set; } = 0;

        public float LevelTime { get; private set; } 
        
        
        public LevelContext(LevelData levelData,
            LevelDifficult levelDifficult,
            GameMode gameMode,
            int neededTargetCount,
            float levelTime)
        {
            LevelData = levelData;
            LevelDifficult = levelDifficult;

            GameMode = gameMode;
            XpPoints = 0;

            NeededTargetCount = neededTargetCount;
            CurrentTargetCount = 0;
            SettedDamage = 0;
            GettedDamage = 0;
            PlayerDeath = 0;
            GameResultStatus = GameResult.None;

            if (levelTime != 0)
                LevelTime = levelTime;
            else LevelTime = -1.0f;

            LevelMarkTypeResult = new Dictionary<LevelMarkType, bool>()
            {
                { LevelMarkType.ClearLevel, false },
                { LevelMarkType.Genocide, false },
                { LevelMarkType.NoDamage, false },
                { LevelMarkType.TimeSuccess, false },
                { LevelMarkType.AllMarksComplete, false },
            };
        }


        public void OnUpdate()
        {
            if (LevelTime > 0)
            {
                LevelTime -= Time.deltaTime;
                if (LevelTime < 0)
                    LevelTime = 0;
            }
        }
        
        public void CompleteLevel(GameResult result)
        {
            GameResultStatus = result;
            
            LevelMarkTypeResult[LevelMarkType.ClearLevel] = (result == GameResult.Victory);
            LevelMarkTypeResult[LevelMarkType.NoDamage] = (GettedDamage == 0);
            LevelMarkTypeResult[LevelMarkType.Genocide] = (NeededTargetCount == CurrentTargetCount);
            if (LevelTime != -1.0f)
            {
                LevelMarkTypeResult[LevelMarkType.TimeSuccess] = LevelTime > 0.0f;
            }
            
            LevelMarkTypeResult[LevelMarkType.AllMarksComplete] =
                LevelMarkTypeResult[LevelMarkType.ClearLevel]
                && LevelMarkTypeResult[LevelMarkType.NoDamage]
                && LevelMarkTypeResult[LevelMarkType.Genocide];

            UpdateScore();
        }
        
        private void UpdateScore()
        {
            foreach (var mark in LevelMarkTypeResult)
            {
                if (mark.Value)
                    XpPoints += MARK_SCORE_COST;
            }

            XpPoints += CurrentTargetCount * TARGET_SCORE_COST;
        }
    }
}