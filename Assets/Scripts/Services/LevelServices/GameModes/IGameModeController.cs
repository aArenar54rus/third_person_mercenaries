namespace Arenar.Services.LevelsService
{
    public interface IGameModeController
    {
        void SetLevelContext(LevelContext levelContext);
        
        void StartGame();

        void EndGame();

        void OnUpdate();
    }
}