using System.Collections.Generic;
using Arenar.Character;

namespace Arenar.Services.LevelsService
{
    public interface IGameModeController
    {
        List<ICharacterEntity> CharacterEntities { get; }
        
        
        void StartGame();

        void EndGame();

        void OnUpdate();
    }
}