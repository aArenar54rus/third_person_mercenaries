namespace Arenar.Services.LevelsService
{
    public interface ILevelsService
    {
        LevelData[] LevelDatas { get; }

        int LastCompleteLevel { get; }

        LevelDifficult LastCompleteDifficult { get; }

        LevelContext CurrentLevelContext { get; }


        bool TryGetLevelDataByIndex(int levelIndex, out LevelData levelData);

        public void LoadLevelScene(int levelIndex, LevelDifficult levelDifficult);

        void SetLevelCompleted();

        void UnloadCurrentLevelScene();
    }
}