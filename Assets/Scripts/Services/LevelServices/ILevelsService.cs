using System;

namespace Arenar.Services.LevelsService
{
    public interface ILevelsService
    {
        event Action<LevelContext> onCompleteLevel;


        LevelData[] LevelDatas { get; }

        int LastCompleteLevel { get; }

        LevelDifficult LastCompleteDifficult { get; }

        LevelContext CurrentLevelContext { get; }


        bool TryGetLevelDataByIndex(int levelIndex, out LevelData levelData);

        public void StartLevel(int levelIndex, LevelDifficult levelDifficult, GameMode gameMode);

        void CompleteLevel();

        void UnloadCurrentLevelScene();
    }
}