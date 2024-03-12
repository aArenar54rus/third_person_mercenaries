namespace Arenar.Services.LevelsService
{
    public interface IGameModeController
    {
        public void StartGame(LevelContext levelContext);

        public void EndGame();

        public void OnUpdate();
    }
}