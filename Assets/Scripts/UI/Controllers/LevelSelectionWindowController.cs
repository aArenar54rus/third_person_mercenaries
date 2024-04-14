using Arenar.Services.LevelsService;
using Arenar.Services.PlayerInputService;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Arenar.Services.UI
{
    public class LevelSelectionWindowController : CanvasWindowController
    {
        private ILevelsService _levelsService;
        private LevelSelectionWindow _levelSelectionWindow;

        private int _currentLevelIndex;
        private GameMode _currentGameMode;
        private LevelDifficult _levelDifficult;

        private LevelSelectionButtonVisual[] _levelSelectionButtonVisuals;


        private LevelSelectionLayer LevelSelectionLayer =>
            _levelSelectionWindow.GetWindowLayer<LevelSelectionLayer>();

        private LevelDifficultLayer LevelDifficultLayer =>
            _levelSelectionWindow.GetWindowLayer<LevelDifficultLayer>();
        

        public LevelSelectionWindowController(ILevelsService levelsService,
            IPlayerInputService playerInputService)
            : base(playerInputService)
        {
            _playerInputService = playerInputService;
            _levelsService = levelsService;
        }

        
        public void OpenWindow()
        {
            OnSelectLevel(0);
            OnLevelDifficult(LevelDifficult.Easy);
        }
        
        public override void Initialize(ICanvasService canvasService)
        {
            base.Initialize(canvasService);

            _levelSelectionWindow = canvasService.GetWindow<LevelSelectionWindow>();
            _levelSelectionWindow.OnShowBegin.AddListener(OpenWindow);

            InitLevelSelectionLayer();
            InitLevelDifficultLayer();

            LevelSelectionLayer.onCanvasLayerShowBegin += OnShowBegin_LoadSelectedLevelData;
            _levelSelectionWindow.OnShowEnd.AddListener(OnWindowShowEnd_SelectElements);
            _levelSelectionWindow.OnHideBegin.AddListener(OnWindowHideBegin_DeselectElements);

            _currentLevelIndex = -1;
        }

        protected override void OnWindowShowEnd_SelectElements()
        {
            if (_levelSelectionButtonVisuals.Length > 0)
                _levelSelectionButtonVisuals[0].Select();
            
            if (_playerInputService.InputActionCollection is PlayerInput playerInput)
                playerInput.UI.Decline.performed += OnInputAction_Decline;
        }

        protected override void OnWindowHideBegin_DeselectElements()
        {
            if (_playerInputService.InputActionCollection is PlayerInput playerInput)
                playerInput.UI.Decline.performed -= OnInputAction_Decline;
        }

        private void OnInputAction_Decline(InputAction.CallbackContext context) =>
            OnReturnToMainMenuButtonClick();

        private void InitLevelSelectionLayer()
        {
            LevelSelectionButtonVisual buttonPrefab = LevelSelectionLayer.LevelSelectionButtonVisualPrefab;
            LevelSelectionLayer.ShowWindowLayerStart();
            LevelSelectionLayer.ShowWindowLayerComplete();

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
            LevelDifficultLayer.BackToMenuButton.onClick.AddListener(OnReturnToMainMenuButtonClick);
        }

        private void OnLevelDifficult(LevelDifficult levelDifficult)
        {
            _levelDifficult = levelDifficult;

            foreach (LevelDifficultButton levelDifficultButton in LevelDifficultLayer.LevelDifficultButtons)
                levelDifficultButton.SetButtonActive(levelDifficultButton.LevelDifficult == _levelDifficult);
            
            foreach (LevelSelectionButtonVisual levelSelectionButton in _levelSelectionButtonVisuals)
                levelSelectionButton.SetDifficultLevel(_levelDifficult);

            if (_levelsService.LastCompleteDifficult > levelDifficult - 1)
            {
                foreach (LevelSelectionButtonVisual levelSelectionButton in _levelSelectionButtonVisuals)
                {
                    levelSelectionButton.SetButtonStatus((levelSelectionButton.LevelData.LevelIndex == _currentLevelIndex)
                        ? LevelSelectionButtonVisual.ButtonStatus.Selected
                        : LevelSelectionButtonVisual.ButtonStatus.Active);
                }
            }
            else if (_levelsService.LastCompleteDifficult < levelDifficult - 1)
            {
                foreach (LevelSelectionButtonVisual levelSelectionButton in _levelSelectionButtonVisuals)
                    levelSelectionButton.SetButtonStatus(LevelSelectionButtonVisual.ButtonStatus.Locked);
            }
            else
            {
                int lastAvailableLevel = _levelsService.LastCompleteLevel;
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

        private void OnSelectLevel(int levelIndex)
        {
            LevelData selectedLevelData = null;
            foreach (LevelSelectionButtonVisual levelSelectionButton in _levelSelectionButtonVisuals)
            {
                if (levelSelectionButton.LevelData.LevelIndex == _currentLevelIndex)
                {
                    levelSelectionButton.SetButtonStatus(LevelSelectionButtonVisual.ButtonStatus.Active);
                }
                else if (levelSelectionButton.LevelData.LevelIndex == levelIndex)
                {
                    levelSelectionButton.SetButtonStatus(LevelSelectionButtonVisual.ButtonStatus.Selected);
                    selectedLevelData = levelSelectionButton.LevelData;
                }
            }

            if (selectedLevelData == null)
                return;
            _currentLevelIndex = selectedLevelData.LevelIndex;
            _currentGameMode = selectedLevelData.GameMode;
        }

        private void OnStartMatchButtonClick()
        {
            _canvasService.TransitionController
                .PlayTransition<TransitionOverlayCanvasWindowController, 
                    LevelSelectionWindow, 
                    GameplayCanvasWindow>(
                        true, 
                        true,
                        () =>
                        {
                            _levelsService.StartLevel(_currentLevelIndex, _levelDifficult, _currentGameMode);
                        });
        }

        private void OnReturnToMainMenuButtonClick()
        {
            _canvasService.TransitionController
                .PlayTransition<TransitionCrossFadeCanvasWindowLayerController, 
                    LevelSelectionWindow, 
                    MainMenuWindow>(
                    false, 
                    false,
                    null);
        }

        private void OnShowBegin_LoadSelectedLevelData(CanvasWindowLayer layer)
        {
            OnSelectLevel(_currentLevelIndex);
            OnLevelDifficult(LevelDifficult.Easy);
        }
    }
}