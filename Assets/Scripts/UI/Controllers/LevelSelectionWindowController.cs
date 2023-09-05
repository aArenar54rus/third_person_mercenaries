using Arenar.Services.LevelsService;
using UnityEngine;
using Zenject;


namespace Arenar.Services.UI
{
    public class LevelSelectionWindowController : CanvasWindowController
    {
        private ILevelsService _levelsService;
        private LevelSelectionWindow _levelSelectionWindow;

        private int _currentLevelIndex;
        private LevelDifficult _levelDifficult;

        private LevelSelectionButtonVisual[] _levelSelectionButtonVisuals;


        private LevelSelectionLayer LevelSelectionLayer =>
            _levelSelectionWindow.GetWindowLayer<LevelSelectionLayer>();

        private LevelDifficultLayer LevelDifficultLayer =>
            _levelSelectionWindow.GetWindowLayer<LevelDifficultLayer>();
        

        public override void Initialize(ICanvasService canvasService)
        {
            base.Initialize(canvasService);

            _levelSelectionWindow = canvasService.GetWindow<LevelSelectionWindow>();

            InitLevelSelectionLayer();
            InitLevelDifficultLayer();

            LevelSelectionLayer.onCanvasLayerShowBegin += OnOpenWindow;
        }

        [Inject]
        public void Construct(ILevelsService levelsService)
        {
            _levelsService = levelsService;
        }

        private void InitLevelSelectionLayer()
        {
            LevelSelectionButtonVisual buttonPrefab = LevelSelectionLayer.LevelSelectionButtonVisualPrefab;

            _levelSelectionButtonVisuals = new LevelSelectionButtonVisual[_levelsService.LevelDatas.Length];
            for (int i = 0; i < _levelsService.LevelDatas.Length; i++)
            {
                _levelSelectionButtonVisuals[i] = Object.Instantiate(buttonPrefab,
                        LevelSelectionLayer.LevelButtonsContainer);
                
                _levelSelectionButtonVisuals[i].Initialize(_levelsService.LevelDatas[i], OnSelectLevel);
            }
        }

        private void InitLevelDifficultLayer()
        {
            foreach (LevelDifficultButton levelDifficultButton in LevelDifficultLayer.LevelDifficultButtons)
                levelDifficultButton.Initialize(OnLevelDifficult);

            LevelDifficultLayer.LevelStartButton.onClick.AddListener(OnStartMatchButtonClick);
        }

        void OnLevelDifficult(LevelDifficult levelDifficult)
        {
            _levelDifficult = levelDifficult;

            foreach (LevelDifficultButton levelDifficultButton in LevelDifficultLayer.LevelDifficultButtons)
                levelDifficultButton.SetButtonActive(levelDifficultButton.LevelDifficult == _levelDifficult);
            
            foreach (LevelSelectionButtonVisual levelSelectionButton in _levelSelectionButtonVisuals)
                levelSelectionButton.SetDifficultLevel(_levelDifficult);

            if (_levelsService.LastCompleteDifficult > levelDifficult)
            {
                foreach (LevelSelectionButtonVisual levelSelectionButton in _levelSelectionButtonVisuals)
                {
                    levelSelectionButton.SetButtonStatus((levelSelectionButton.LevelData.LevelIndex == _currentLevelIndex)
                        ? LevelSelectionButtonVisual.ButtonStatus.Selected
                        : LevelSelectionButtonVisual.ButtonStatus.Active);
                }
            }
            else if (_levelsService.LastCompleteDifficult < levelDifficult)
            {
                foreach (LevelSelectionButtonVisual levelSelectionButton in _levelSelectionButtonVisuals)
                    levelSelectionButton.SetButtonStatus(LevelSelectionButtonVisual.ButtonStatus.Locked);
            }
            else
            {
                int lastAvailableLevel = _levelsService.LastCompleteLevel + 1;
                foreach (LevelSelectionButtonVisual levelSelectionButton in _levelSelectionButtonVisuals)
                {
                    if (levelSelectionButton.LevelData.LevelIndex >= lastAvailableLevel)
                    {
                        levelSelectionButton.SetButtonStatus((levelSelectionButton.LevelData.LevelIndex == _currentLevelIndex)
                            ? LevelSelectionButtonVisual.ButtonStatus.Selected
                            : LevelSelectionButtonVisual.ButtonStatus.Active);
                    }
                    else
                    {
                        levelSelectionButton.SetButtonStatus(LevelSelectionButtonVisual.ButtonStatus.Locked);
                    }
                }
            }
        }

        void OnSelectLevel(int levelIndex)
        {
            foreach (LevelSelectionButtonVisual levelSelectionButton in _levelSelectionButtonVisuals)
            {
                if (levelSelectionButton.LevelData.LevelIndex == _currentLevelIndex)
                    levelSelectionButton.SetButtonStatus(LevelSelectionButtonVisual.ButtonStatus.Active);
                else if (levelSelectionButton.LevelData.LevelIndex == levelIndex)
                    levelSelectionButton.SetButtonStatus(LevelSelectionButtonVisual.ButtonStatus.Selected);
            }
                
            _currentLevelIndex = levelIndex;
        }

        private void OnStartMatchButtonClick()
        {
            _canvasService.TransitionController
                .PlayTransition<
                    TransitionOverlayCanvasWindowController, 
                    LevelSelectionWindow, 
                    GameplayCanvasWindow>(
                        true, 
                        true,
                        () =>
                        {
                            _levelsService.LoadLevelScene(_currentLevelIndex, _levelDifficult);
                        });
        }

        private void OnOpenWindow(CanvasWindowLayer layer)
        {
            OnSelectLevel(_currentLevelIndex);
            OnLevelDifficult(LevelDifficult.Easy);
        }
    }
}