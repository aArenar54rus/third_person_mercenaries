using System;
using Arenar.Character;
using Arenar.Services.SaveAndLoad;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;


namespace Arenar.Services.LevelsService
{
    public class LevelsService : ILevelsService
    {
        public event Action<LevelContext> onCompleteLevel;
        
        
        private int _lastCompleteLevel;
        private LevelDifficult _lastCompleteDifficult;

        private ZenjectSceneLoader _sceneLoader;
        private ISaveAndLoadService<SaveDelegate> _saveAndLoadService;
        private LevelData[] _levelDatas;

        private GameModeController _gameModeController;

        private CharacterSpawnController _сharacterSpawnController;
        private ShootingGalleryLevelInfoCollection _shootingGalleryLevelInfoCollection;

        private TickableManager _tickableManager;

        
        public LevelData[] LevelDatas => _levelDatas;
        public int LastCompleteLevel => _lastCompleteLevel;
        public LevelDifficult LastCompleteDifficult => _lastCompleteDifficult;
        public LevelContext CurrentLevelContext { get; private set; }


        [Inject]
        public void Construct(ZenjectSceneLoader sceneLoader,
                              ISaveAndLoadService<SaveDelegate> saveAndLoadService,
                              LevelData[] levelDatas,
                              CharacterSpawnController сharacterSpawnController,
                              TickableManager tickableManager,
                              ShootingGalleryLevelInfoCollection shootingGalleryLevelInfoCollection)
        {
            _sceneLoader = sceneLoader;
            _saveAndLoadService = saveAndLoadService;
            _levelDatas = levelDatas;
            _сharacterSpawnController = сharacterSpawnController;
            _tickableManager = tickableManager;
            _shootingGalleryLevelInfoCollection = shootingGalleryLevelInfoCollection;
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
                _shootingGalleryLevelInfoCollection.ShootingGalleriesInfos[levelIndex].Length);
            
            _sceneLoader.LoadScene(CurrentLevelContext.LevelData.SceneKey, LoadSceneMode.Additive);

            _gameModeController = CreateGame();
            _gameModeController.StartGame();
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
                        ShootingGalleryGameModeController shootingGalleryGameMode = new(_сharacterSpawnController);
                        shootingGalleryGameMode.Initialize(_shootingGalleryLevelInfoCollection.ShootingGalleriesInfos[levelIndex]);
                        shootingGalleryGameMode.SetLevelContext(CurrentLevelContext);
                        return shootingGalleryGameMode;
                    
                    default:
                        Debug.LogError($"Unknown gameMode {gameMode}");
                        return null;
                }
            }
        }

        public void CompleteLevel()
        {
            if (CurrentLevelContext == null
                || CurrentLevelContext.LevelData.LevelIndex <= _lastCompleteLevel)
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
                if (_lastCompleteLevel == _levelDatas.Length - 1)
                {
                    _lastCompleteLevel = 0;
                    if (_lastCompleteDifficult != LevelDifficult.Infinity)
                        _lastCompleteDifficult++;
                }

                LevelProgressionSaveDelegate saveDelegate = new LevelProgressionSaveDelegate
                {
                    completeDifficult = CurrentLevelContext.LevelDifficult,
                    completedLevel = _lastCompleteLevel
                };
                _saveAndLoadService.MakeSave(saveDelegate);
            }

            _tickableManager.Remove(_gameModeController);
            _gameModeController.EndGame();
            
            onCompleteLevel?.Invoke(CurrentLevelContext);
        }

        public void UnloadCurrentLevelScene()
        {
            if (CurrentLevelContext != null)
                SceneManager.UnloadSceneAsync(CurrentLevelContext.LevelData.LevelNameKey);
        }
    }
}