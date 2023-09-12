using Arenar.Services.SaveAndLoad;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;


namespace Arenar.Services.LevelsService
{
    public class LevelsService : ILevelsService
    {
        private int _lastCompleteLevel;
        private LevelDifficult _lastCompleteDifficult;

        private ZenjectSceneLoader _sceneLoader;
        private ISaveAndLoadService<SaveDelegate> _saveAndLoadService;
        private LevelData[] _levelDatas;
        

        public LevelData[] LevelDatas => _levelDatas;
        public int LastCompleteLevel => _lastCompleteLevel;
        public LevelDifficult LastCompleteDifficult => _lastCompleteDifficult;
        public LevelContext CurrentLevelContext { get; private set; }


        [Inject]
        public void Construct(ZenjectSceneLoader sceneLoader,
                              ISaveAndLoadService<SaveDelegate> saveAndLoadService,
                              LevelData[] levelDatas)
        {
            _sceneLoader = sceneLoader;
            _saveAndLoadService = saveAndLoadService;
            _levelDatas = levelDatas;
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

        public void LoadLevelScene(int levelIndex, LevelDifficult levelDifficult)
        {
            if (!TryGetLevelDataByIndex(levelIndex, out LevelData levelData))
            {
                Debug.LogError("Unknown level for load!");
                return;
            }
            
            CurrentLevelContext = new LevelContext
            {
                LevelData = levelData,
                LevelDifficult = levelDifficult
            };
            
            _sceneLoader.LoadScene(CurrentLevelContext.LevelData.SceneKey, LoadSceneMode.Additive);
        }

        public void SetLevelCompleted()
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
            saveDelegate.completeDifficult = LevelDifficult.Infinity;
            saveDelegate.completedLevel = _lastCompleteLevel;
            _saveAndLoadService.MakeSave(saveDelegate);
        }

        public void UnloadCurrentLevelScene()
        {
            if (CurrentLevelContext != null)
                SceneManager.UnloadSceneAsync(CurrentLevelContext.LevelData.LevelNameKey);
        }
    }
}