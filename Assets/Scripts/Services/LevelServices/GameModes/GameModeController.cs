namespace Arenar.Services.LevelsService
{
    public abstract class GameModeController : IGameModeController
    {
        public System.Action onGameComplete;
        
        
        public virtual void StartGame(ILevelsService levelService)
        {
            
        }

        public abstract void EndGame();

        public virtual void OnUpdate() { }
    }
}