using System.Collections.Generic;
using Arenar.Character;

namespace Arenar.Services.LevelsService
{
    public abstract class GameModeController : IGameModeController
    {
        public System.Action onGameComplete;
        
        
        public List<ICharacterEntity> CharacterEntities { get; protected set; }


        public abstract void StartGame();

        public abstract void EndGame();

        public virtual void OnUpdate() { }
    }
}