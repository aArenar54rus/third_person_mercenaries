using System;
using Arenar.Services.SaveAndLoad;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;


namespace Arenar.Services.LevelsService
{
    public class LevelsService : ILevelsService, ITickable
    {
        public event Action<LevelContext> onCompleteLevel;
        
        
        private int _lastCompleteLevel;
        private LevelDifficult _lastCompleteDifficult;

        private ZenjectSceneLoader _sceneLoader;
        private ISaveAndLoadService<SaveDelegate> _saveAndLoadService;
        private LevelData[] _levelDatas;

        private GameModeController _gameModeController;

        private CharacterSpawnController _сharacterSpawnController;


        
        public LevelData[] LevelDatas => _levelDatas;
        public int LastCompleteLevel => _lastCompleteLevel;
        public LevelDifficult LastCompleteDifficult => _lastCompleteDifficult;
        public LevelContext CurrentLevelContext { get; private set; }


        [Inject]
        public void Construct(ZenjectSceneLoader sceneLoader,
                              ISaveAndLoadService<SaveDelegate> saveAndLoadService,
                              LevelData[] levelDatas,
                              CharacterSpawnController сharacterSpawnController)
        {
            _sceneLoader = sceneLoader;
            _saveAndLoadService = saveAndLoadService;
            _levelDatas = levelDatas;
            _сharacterSpawnController = сharacterSpawnController;
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

            CurrentLevelContext = new LevelContext(levelData, levelDifficult, gameMode);
            
            _sceneLoader.LoadScene(CurrentLevelContext.LevelData.SceneKey, LoadSceneMode.Additive);
            
            _gameModeController = CreateGameModeController();
            _gameModeController.StartGame();


            GameModeController CreateGameModeController()
            {
                switch (gameMode)
                {
                    case GameMode.Campaing:
                        return null;
                    
                    case GameMode.Survival:
                        return null;
                    
                    case GameMode.ShootingGallery:
                        ShootingGalleryGameModeController shootingGalleryGameMode = new(_сharacterSpawnController);
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
                return;

            _lastCompleteLevel = CurrentLevelContext.LevelData.LevelIndex;
            if (_lastCompleteLevel == _levelDatas.Length - 1)
            {
                _lastCompleteLevel = 0;
                if (_lastCompleteDifficult != LevelDifficult.Infinity)
                    _lastCompleteDifficult++;
            }

            var saveDelegate = new LevelProgressionSaveDelegate();
            saveDelegate.completeDifficult = CurrentLevelContext.LevelDifficult;
            saveDelegate.completedLevel = _lastCompleteLevel;
            _saveAndLoadService.MakeSave(saveDelegate);
            
            _gameModeController.EndGame();
            
            onCompleteLevel?.Invoke(CurrentLevelContext);
        }

        public void UnloadCurrentLevelScene()
        {
            if (CurrentLevelContext != null)
                SceneManager.UnloadSceneAsync(CurrentLevelContext.LevelData.LevelNameKey);
        }

        public void Tick()
        {
            if (_gameModeController == null)
                return;
            
            _gameModeController.OnUpdate();
        }
    }
}