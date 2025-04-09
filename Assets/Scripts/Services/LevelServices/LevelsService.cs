using System;
using Arenar.CameraService;
using Arenar.Character;
using Arenar.LocationService;
using Arenar.Services.SaveAndLoad;
using TakeTop.PreferenceSystem;
using UnityEngine;
using Zenject;


namespace Arenar.Services.LevelsService
{
    public class LevelsService : ILevelsService
    {
        public event Action<LevelContext> onCompleteLevel;
        
        
        private int _lastCompleteLevel = 0;
        private LevelDifficult _lastCompleteDifficult;

        private ILocationService locationService;
        private IPreferenceManager preferenceManager;
        private LevelData[] levelDatas;

        private GameModeController _gameModeController;

        private ICameraService cameraService;
        private CharacterSpawnController сharacterSpawnController;
        private ClearLocationLevelInfoCollection clearLocationLevelInfoCollection;
        private SurvivalLevelInfoCollection survivalLevelInfoCollection;
        private PlayerCharacterParametersUpgradeService playerCharacterParametersUpgradeService;

        private TickableManager tickableManager;

        
        public LevelData[] LevelDatas => levelDatas;
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
                              ClearLocationLevelInfoCollection clearLocationLevelInfoCollection,
                              SurvivalLevelInfoCollection survivalLevelInfoCollection,
                              PlayerCharacterParametersUpgradeService playerCharacterParametersUpgradeService)
        {
            this.locationService = locationService;
            this.preferenceManager = preferenceManager;
            this.levelDatas = levelDatas;
            this.сharacterSpawnController = сharacterSpawnController;
            this.tickableManager = tickableManager;
            this.clearLocationLevelInfoCollection = clearLocationLevelInfoCollection;
            this.survivalLevelInfoCollection = survivalLevelInfoCollection;
            this.playerCharacterParametersUpgradeService = playerCharacterParametersUpgradeService;
            
            this.cameraService = cameraService;
            
            var playerSaveData = this.preferenceManager.LoadValue<LevelProgressionSaveDelegate>();
            _lastCompleteLevel = playerSaveData.completedLevel;
            _lastCompleteDifficult = playerSaveData.completeDifficult;
        }
        
        public bool TryGetLevelDataByIndex(int levelIndex, out LevelData levelData)
        {
            foreach (var ld in levelDatas)
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
                clearLocationLevelInfoCollection.ShootingGalleriesInfos[levelIndex].Length,
                clearLocationLevelInfoCollection.LevelTime);
            
            locationService.ChangeLoadedScene(levelData.LocationName,
                (sceneContainer) =>
                {
                    _gameModeController = CreateGame(sceneContainer);
                    _gameModeController.StartGame();
                    _gameModeController.onGameComplete += CompleteLevel;
                    tickableManager.Add(_gameModeController);
                });
            

            GameModeController CreateGame(DiContainer sceneContainer)
            {
                switch (gameMode)
                {
                    case GameMode.Campaing:
                        // ClearLocationGameModeController clearCampaingLocationGameMode = new(сharacterSpawnController, cameraService);

                        return null;
                    
                    case GameMode.Survival:
                        // PlayerSpawnPoint spawnPoint = container.Resolve<PlayerSpawnPoint>();
                        // EnemySpawnPoints enemySpawnPoints = container.Resolve<EnemySpawnPoints>();
                        SurvivalGameModeController survivalGameModeController = new SurvivalGameModeController(
                            this,
                            сharacterSpawnController,
                            cameraService,
                            survivalLevelInfoCollection,
                            sceneContainer,
                            playerCharacterParametersUpgradeService
                        );

                        survivalGameModeController.SetLevelContext(CurrentLevelContext);
                        
                        return survivalGameModeController;
                    
                    case GameMode.ShootingGallery:
                        ClearLocationGameModeController clearLocationGameMode = new(сharacterSpawnController, cameraService);
                        clearLocationGameMode.Initialize(clearLocationLevelInfoCollection.ShootingGalleriesInfos[levelIndex]);
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

            if (!сharacterSpawnController.PlayerCharacter
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
                var playerSaveData = preferenceManager.LoadValue<LevelProgressionSaveDelegate>();
                if (_lastCompleteLevel >= levelDatas.Length - 1)
                {
                    _lastCompleteLevel = -1;
                    if (_lastCompleteDifficult != LevelDifficult.Infinity)
                    {
                        _lastCompleteDifficult++;
                        playerSaveData.completeDifficult++;
                    }
                }
                
                playerSaveData.completedLevel = _lastCompleteLevel;
                preferenceManager.SaveValue(playerSaveData);
            }

            tickableManager.Remove(_gameModeController);
            _gameModeController.EndGame();
            
            onCompleteLevel?.Invoke(CurrentLevelContext);
        }

        public void UnloadCurrentLevelScene()
        {
            if (CurrentLevelContext != null)
            {
                locationService.ChangeLoadedScene(LocationName.MainMenuLocation);
            }
        }
    }
}