using Zenject;

namespace Arenar.Services.LevelsService
{
    public abstract class GameModeController : IGameModeController, ITickable
    {
        public System.Action onGameComplete;
        
        
        protected LevelContext _levelContext;


        public void SetLevelContext(LevelContext levelContext) =>
            _levelContext = levelContext;

        public abstract void StartGame();

        public abstract void EndGame();

        public virtual void OnUpdate() { }
        
        public void Tick()
        {
            OnUpdate();
            _levelContext.OnUpdate();
        }

        protected int GetRandomEnemyLevel()
        {
            var enemyLevels = _levelContext.LevelData.DifficultData[_levelContext.LevelDifficult];
            return UnityEngine.Random.Range(enemyLevels.MinimalEnemyLevel, enemyLevels.MaximumEnemyLevel);
        }
    }
}