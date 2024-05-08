using Arenar.AudioSystem;
using Arenar.Services.LevelsService;
using Arenar.Services.PlayerInputService;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


namespace Arenar.Services.UI
{
    public class LevelSelectionWindowController : CanvasWindowController
    {
        private const int ADDED_WIGHT = 50;
        
        private ILevelsService _levelsService;
        private LevelSelectionWindow _levelSelectionWindow;

        private int _currentLevelIndex;
        private GameMode _currentGameMode;
        private LevelDifficult _levelDifficult;

        private ICanvasService _canvasService;
        private IAmbientManager _ambientManager;
        private IUiSoundManager _uiSoundManager;

        private LevelSelectionButtonVisual[] _levelSelectionButtonVisuals;


        private LevelSelectionLayer LevelSelectionLayer =>
            _levelSelectionWindow.GetWindowLayer<LevelSelectionLayer>();

        private LevelDifficultLayer LevelDifficultLayer =>
            _levelSelectionWindow.GetWindowLayer<LevelDifficultLayer>();
        

        public LevelSelectionWindowController(ILevelsService levelsService,
            IPlayerInputService playerInputService,
            IUiSoundManager uiSoundManager,
            IAmbientManager ambientManager,
            ICanvasService canvasService)
            : base(playerInputService)
        {
            _playerInputService = playerInputService;
            _levelsService = levelsService;
            _uiSoundManager = uiSoundManager;
            _ambientManager = ambientManager;
            _canvasService = canvasService;
        }
        
        
        public override void Initialize(ICanvasService canvasService)
        {
            base.Initialize(canvasService);

            _levelSelectionWindow = canvasService.GetWindow<LevelSelectionWindow>();

            InitLevelSelectionLayer();
            InitLevelDifficultLayer();

            LevelSelectionLayer.onCanvasLayerShowBegin += OnShowBegin_LoadSelectedLevelData;
            _levelSelectionWindow.OnShowEnd.AddListener(OnWindowShowEnd_SelectElements);
            _levelSelectionWindow.OnHideBegin.AddListener(OnWindowHideBegin_DeselectElements);

            _currentLevelIndex = 0;
        }

        protected override void OnWindowShowEnd_SelectElements()
        {
            if (_levelSelectionButtonVisuals.Length > 0)
                _levelSelectionButtonVisuals[0].Select();
            
            if (_playerInputService.InputActionCollection is PlayerInput playerInput)
                playerInput.UI.Decline.performed += OnInputAction_Decline;

            SetButtonsStatus(true);
        }

        protected override void OnWindowHideBegin_DeselectElements()
        {
            if (_playerInputService.InputActionCollection is PlayerInput playerInput)
                playerInput.UI.Decline.performed -= OnInputAction_Decline;
            
            SetButtonsStatus(false);
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

            ChangeContainerWidth();
            
            
            void ChangeContainerWidth()
            {
                float levelSelectionButtonLenght =
                    _levelSelectionButtonVisuals[0].GetComponent<RectTransform>().rect.width;

                float spacing = LevelSelectionLayer.LevelButtonsContainer.GetComponent<HorizontalLayoutGroup>().spacing;

                float width = levelSelectionButtonLenght * _levelSelectionButtonVisuals.Length
                              + spacing * (_levelSelectionButtonVisuals.Length - 1) + ADDED_WIGHT;
                width *= _canvasService.RootCanvas.scaleFactor;

                RectTransform levelButtonsContainerTransform =
                    LevelSelectionLayer.LevelButtonsContainer.GetComponent<RectTransform>();
                levelButtonsContainerTransform.sizeDelta = new Vector2(width, levelButtonsContainerTransform.sizeDelta.y);
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
                    levelSelectionButton.SetButtonStatus(LevelSelectionButtonVisual.ButtonStatus.Active);

                OnSelectLevel(Mathf.Min(_levelsService.LastCompleteLevel + 1, _levelSelectionButtonVisuals.Length));
            }
            else if (_levelsService.LastCompleteDifficult < levelDifficult - 1)
            {
                foreach (LevelSelectionButtonVisual levelSelectionButton in _levelSelectionButtonVisuals)
                    levelSelectionButton.SetButtonStatus(LevelSelectionButtonVisual.ButtonStatus.Locked);
            }
            else
            {
                foreach (LevelSelectionButtonVisual levelSelectionButton in _levelSelectionButtonVisuals)
                {
                    levelSelectionButton.SetButtonStatus(
                        (levelSelectionButton.LevelData.LevelIndex <= _levelsService.LastCompleteLevel + 1)
                            ? LevelSelectionButtonVisual.ButtonStatus.Active
                            : LevelSelectionButtonVisual.ButtonStatus.Locked);
                }
                
                OnSelectLevel(Mathf.Min(_levelsService.LastCompleteLevel + 1, _levelSelectionButtonVisuals.Length));
            }
        }

        private void OnSelectLevel(int levelIndex)
        {
            LevelData selectedLevelData = null;
            foreach (LevelSelectionButtonVisual levelSelectionButton in _levelSelectionButtonVisuals)
            {
                if (levelSelectionButton.LevelData.LevelIndex == _currentLevelIndex && _currentLevelIndex != levelIndex)
                {
                    levelSelectionButton.SetButtonStatus(LevelSelectionButtonVisual.ButtonStatus.Active);
                    continue;
                }
                
                if (levelSelectionButton.LevelData.LevelIndex == levelIndex)
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
            _ambientManager.StopAmbient();
            _uiSoundManager.PlaySound(UiSoundType.StandartButtonClick);
            _canvasService.TransitionController
                .PlayTransition<TransitionOverlayCanvasWindowController, 
                    LevelSelectionWindow, 
                    GameplayCanvasWindow>(
                        true, 
                        true,
                        () =>
                        {
                            _levelsService.StartLevel(_currentLevelIndex, _levelDifficult, _currentGameMode);
                            _ambientManager.PlayAmbient(AmbientType.Gameplay);
                        });
        }

        private void OnReturnToMainMenuButtonClick()
        {
            _uiSoundManager.PlaySound(UiSoundType.StandartButtonClick);
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
            OnLevelDifficult(LevelDifficult.Easy);
        }
        
        private void SetButtonsStatus(bool status)
        {
            foreach (LevelSelectionButtonVisual levelSelectionButton in _levelSelectionButtonVisuals)
            {
                levelSelectionButton.Interactable = status;
            }

            foreach (LevelDifficultButton levelDifficultButton in LevelDifficultLayer.LevelDifficultButtons)
                levelDifficultButton.Interactable = status;

            LevelDifficultLayer.LevelStartButton.interactable = status;
            LevelDifficultLayer.BackToMenuButton.interactable = status;
        }
    }
}