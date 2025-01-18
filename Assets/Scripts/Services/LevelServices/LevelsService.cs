using System;
using Arenar.CameraService;
using Arenar.Character;
using Arenar.LocationService;
using Arenar.Services.SaveAndLoad;
using TakeTop.PreferenceSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;


namespace Arenar.Services.LevelsService
{
    public class LevelsService : ILevelsService
    {
        public event Action<LevelContext> onCompleteLevel;
        
        
        private int _lastCompleteLevel = 0;
        private LevelDifficult _lastCompleteDifficult;

        private ILocationService _locationService;
        private IPreferenceManager _preferenceManager;
        private LevelData[] _levelDatas;

        private GameModeController _gameModeController;

        private ICameraService _cameraService;
        private CharacterSpawnController _сharacterSpawnController;
        private ShootingGalleryLevelInfoCollection _shootingGalleryLevelInfoCollection;

        private TickableManager _tickableManager;

        
        public LevelData[] LevelDatas => _levelDatas;
        public int LastCompleteLevel => _lastCompleteLevel;
        public LevelDifficult LastCompleteDifficult => _lastCompleteDifficult;
        public LevelContext CurrentLevelContext { get; private set; }


        [Inject]
        public void Construct(ILocationService locationService,
                              IPreferenceManager preferenceManager,
                              LevelData[] levelDatas,
                              CharacterSpawnController сharacterSpawnController,
                              TickableManager tickableManager,
                              ICameraService cameraService,
                              ShootingGalleryLevelInfoCollection shootingGalleryLevelInfoCollection)
        {
            _locationService = locationService;
            _preferenceManager = preferenceManager;
            _levelDatas = levelDatas;
            _сharacterSpawnController = сharacterSpawnController;
            _tickableManager = tickableManager;
            _shootingGalleryLevelInfoCollection = shootingGalleryLevelInfoCollection;
            _cameraService = cameraService;
            
            var playerSaveData = _preferenceManager.LoadValue<LevelProgressionSaveDelegate>();
            _lastCompleteLevel = playerSaveData.completedLevel;
            _lastCompleteDifficult = playerSaveData.completeDifficult;
        }
        
        public bool TryGetLevelDataByIndex(int levelIndex, out LevelData levelData)
        {
            foreach (var ld in _levelDatas)
            {
                if (ld.LevelIndex != levelIndex)
                    continue;
                levelData = ld;
                return true;
            }

            levelData = null;
            return false;
        }

        public void StartLevel(int levelIndex, LevelDifficult levelDifficult, GameMode gameMode)
        {
            if (!TryGetLevelDataByIndex(levelIndex, out LevelData levelData))
            {
                Debug.LogError("Unknown level for load!");
                return;
            }

            CurrentLevelContext = new LevelContext(levelData,
                levelDifficult,
                gameMode,
                _shootingGalleryLevelInfoCollection.ShootingGalleriesInfos[levelIndex].Length,
                _shootingGalleryLevelInfoCollection.LevelTime);
            
            _locationService.ChangeLoadedScene(levelData.LocationName);

            _gameModeController = CreateGame();
            _gameModeController.StartGame();
            _gameModeController.onGameComplete += CompleteLevel;
            _tickableManager.Add(_gameModeController);


            GameModeController CreateGame()
            {
                switch (gameMode)
                {
                    case GameMode.Campaing:
                        return null;
                    
                    case GameMode.Survival:
                        return null;
                    
                    case GameMode.ShootingGallery:
                        ClearLocationGameModeController clearLocationGameMode = new(_сharacterSpawnController, _cameraService);
                        clearLocationGameMode.Initialize(_shootingGalleryLevelInfoCollection.ShootingGalleriesInfos[levelIndex]);
                        clearLocationGameMode.SetLevelContext(CurrentLevelContext);
                        return clearLocationGameMode;
                    
                    default:
                        Debug.LogError($"Unknown gameMode {gameMode}");
                        return null;
                }
            }
        }

        public void CompleteLevel()
        {
            _gameModeController.onGameComplete -= CompleteLevel;
            
            if (CurrentLevelContext == null)
            {
                Debug.LogError("Data is lost!");
                return;
            }

            if (!_сharacterSpawnController.PlayerCharacter
                    .TryGetCharacterComponent<ICharacterLiveComponent>(out ICharacterLiveComponent liveComponent))
            {
                return;
            }

            CurrentLevelContext.CompleteLevel(liveComponent.IsAlive
                ? LevelContext.GameResult.Victory
                : LevelContext.GameResult.Defeat);
            
            if (CurrentLevelContext.GameResultStatus == LevelContext.GameResult.Victory)
            {
                _lastCompleteLevel = CurrentLevelContext.LevelData.LevelIndex;
                var playerSaveData = _preferenceManager.LoadValue<LevelProgressionSaveDelegate>();
                if (_lastCompleteLevel >= _levelDatas.Length - 1)
                {
                    _lastCompleteLevel = -1;
                    if (_lastCompleteDifficult != LevelDifficult.Infinity)
                    {
                        _lastCompleteDifficult++;
                        playerSaveData.completeDifficult++;
                    }
                }
                
                playerSaveData.completedLevel = _lastCompleteLevel;
                _preferenceManager.SaveValue(playerSaveData);
            }

            _tickableManager.Remove(_gameModeController);
            _gameModeController.EndGame();
            
            onCompleteLevel?.Invoke(CurrentLevelContext);
        }

        public void UnloadCurrentLevelScene()
        {
            if (CurrentLevelContext != null)
            {
                _locationService.ChangeLoadedScene(LocationName.MainMenuLocation);
            }
        }
    }
}