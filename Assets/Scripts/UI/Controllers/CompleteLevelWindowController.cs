using Arenar.AudioSystem;
using Arenar.CameraService;
using Arenar.Character;
using Arenar.LocationService;
using Arenar.Services.LevelsService;
using Arenar.Services.PlayerInputService;
using Arenar.Services.SaveAndLoad;
using TakeTop.PreferenceSystem;
using UnityEngine.InputSystem;
using YG;
using Zenject;


namespace Arenar.Services.UI
{
    public class CompleteLevelWindowController : CanvasWindowController
    {
        private ILevelsService _levelsService;
        
        private CompleteLevelCanvasWindow _completeLevelCanvasWindow;
        private CompleteLevelLayer _completeLevelLayer;
        private FailedLevelLayer _failedLevelLayer;
        private PlayerCharacterLevelData _playerCharacterLevelData;
        private LevelContext _lastLevelContext;

        private ICameraService _cameraService;
        private CharacterSpawnController _сharacterSpawnController;
        private IPreferenceManager _preferenceManager;
        private IUiSoundManager _uiSoundManager;
        private ILocationService _locationService;

        private PlayerSaveDelegate _playerSaveDelegate;
        private bool _isPlayerLevelMax;

        private YandexGamesAdsService _yandexGamesAdsService;


        private ProgressLevelMarksVisualControl ProgressLevelMarksVisualControl =>
            _completeLevelLayer.ProgressLevelMarksVisualControl;


        public CompleteLevelWindowController(ILevelsService levelsService,
            IPlayerInputService playerInputService,
            PlayerCharacterLevelData playerCharacterLevelData,
            IPreferenceManager preferenceManager,
            ICameraService cameraService,
            IUiSoundManager uiSoundManager,
            ILocationService locationService,
            CharacterSpawnController сharacterSpawnController,
            YandexGamesAdsService yandexGamesAdsService)
            : base(playerInputService)
        {
            _playerInputService = playerInputService;
            _levelsService = levelsService;
            _playerCharacterLevelData = playerCharacterLevelData;
            _preferenceManager = preferenceManager;
            _cameraService = cameraService;
            _сharacterSpawnController = сharacterSpawnController;
            _uiSoundManager = uiSoundManager;
            _locationService = locationService;
            _yandexGamesAdsService = yandexGamesAdsService;
        }


        public override void Initialize(ICanvasService canvasService)
        {
            base.Initialize(canvasService);
            
            _completeLevelCanvasWindow = _canvasService.GetWindow<CompleteLevelCanvasWindow>();
            _completeLevelLayer = _completeLevelCanvasWindow.GetWindowLayer<CompleteLevelLayer>();
            _failedLevelLayer = _completeLevelCanvasWindow.GetWindowLayer<FailedLevelLayer>();
            
            _completeLevelLayer.ContinueButton.onClick.AddListener(OnContinueButtonClick);
            _completeLevelLayer.AdRewardButton.onClick.AddListener(OnContinueButtonClickRewarded);
            _failedLevelLayer.ReturnInMainMenuButton.onClick.AddListener(OnContinueButtonClick);

            _levelsService.onCompleteLevel += OnLevelComplete;
            _completeLevelCanvasWindow.OnShowEnd.AddListener(OnWindowShowEnd_SelectElements);
            _completeLevelCanvasWindow.OnHideBegin.AddListener(OnWindowHideBegin_DeselectElements);
        }

        protected override void OnWindowShowEnd_SelectElements()
        {
            if (_lastLevelContext.GameResultStatus == LevelContext.GameResult.Victory)
                _completeLevelLayer.AdRewardButton.Select();
            else
                _failedLevelLayer.ReturnInMainMenuButton.Select();

            if (_playerInputService.InputActionCollection is PlayerInput playerInput)
            {
                _playerInputService.SetInputControlType(InputActionMapType.UI, true);        
                _playerInputService.SetInputControlType(InputActionMapType.Gameplay, false);        
                playerInput.UI.Decline.performed += OnInputAction_Decline;
            }

            SetButtonsStatus(true);
        }

        protected override void OnWindowHideBegin_DeselectElements()
        {
            if (_playerInputService.InputActionCollection is PlayerInput playerInput)
                playerInput.UI.Decline.performed -= OnInputAction_Decline;

            SetButtonsStatus(false);
        }

        private void OnInputAction_Decline(InputAction.CallbackContext callbackContext) =>
            OnContinueButtonClick();

        private void OnContinueButtonClick()
        {
            _uiSoundManager.PlaySound(UiSoundType.StandartButtonClick);
            if (!_isPlayerLevelMax)
                UpdateReward(1);
            
            _yandexGamesAdsService.ShowInterstitial();
            ReturnInMainMenu();
        }

        private void OnContinueButtonClickRewarded()
        {
            _uiSoundManager.PlaySound(UiSoundType.StandartButtonClick);
            _completeLevelLayer.CompleteLevelItemRewardsVisualControl.SetXpValue(_lastLevelContext.XpPoints * 3);
            if (!_isPlayerLevelMax)
                UpdateReward(3);
            
            _yandexGamesAdsService.ShowRewarded();
            ReturnInMainMenu();
        }

        private void UpdateReward(int multiplier)
        {
            int currentPlayerExperience = _playerSaveDelegate.currentXpPoints + _lastLevelContext.XpPoints * multiplier;
            int currentLevelExpMax = _playerCharacterLevelData
                .GetExperienceForNextLevel(_playerSaveDelegate.playerCharacterLevel);
            
            while (currentPlayerExperience > currentLevelExpMax)
            {
                currentPlayerExperience -= currentLevelExpMax;
                _playerSaveDelegate.playerCharacterLevel++;
                    
                _isPlayerLevelMax = (_playerCharacterLevelData.MaxLevel == _playerSaveDelegate.playerCharacterLevel);
                if (_isPlayerLevelMax)
                    break;
                    
                currentLevelExpMax = _playerCharacterLevelData
                    .GetExperienceForNextLevel(_playerSaveDelegate.playerCharacterLevel);
            }
            
            _playerSaveDelegate.currentXpPoints = currentPlayerExperience;
            _preferenceManager.SaveValue<PlayerSaveDelegate>(_playerSaveDelegate);
        }

        private void ReturnInMainMenu()
        {
            _canvasService.TransitionController
                .PlayTransition<TransitionOverlayCanvasWindowController,
                        CompleteLevelCanvasWindow,
                        MainMenuWindow>
                    (true, true, () =>
                    {
                        _сharacterSpawnController.DisableAllCharacters();
                        _locationService.UnloadLastLoadedLocation();
                        _locationService.LoadLocation(LocationName.MainMenuLocation);
                        
                        _cameraService.SetCameraState<CameraStateMainMenu>(null, null);
                    });
        }
        
        private void UpdateMarkSuccessStatus(LevelMarkType type) =>
            ProgressLevelMarksVisualControl.SetMarkSuccessStatus(type, _lastLevelContext.LevelMarkTypeResult[type]);

        private void OnLevelComplete(LevelContext levelContext)
        {
            _playerSaveDelegate = _preferenceManager.LoadValue<PlayerSaveDelegate>();
            _isPlayerLevelMax = (_playerCharacterLevelData.MaxLevel == _playerSaveDelegate.playerCharacterLevel);
            _lastLevelContext = levelContext;
            
            if (levelContext.GameResultStatus == LevelContext.GameResult.Victory)
            {
                _completeLevelLayer.gameObject.SetActive(true);
                _failedLevelLayer.gameObject.SetActive(false);

                UpdateMarkSuccessStatus(LevelMarkType.ClearLevel);
                UpdateMarkSuccessStatus(LevelMarkType.Genocide);
                UpdateMarkSuccessStatus(LevelMarkType.NoDamage);
                UpdateMarkSuccessStatus(LevelMarkType.AllMarksComplete);
                
                _completeLevelLayer.CompleteLevelItemRewardsVisualControl.ClearItemRewards();
                _completeLevelLayer.CompleteLevelItemRewardsVisualControl.SetXpValue(_lastLevelContext.XpPoints);
            }
            else
            {
                _completeLevelLayer.gameObject.SetActive(false);
                _failedLevelLayer.gameObject.SetActive(true);
                _failedLevelLayer.CompleteLevelItemRewardsVisualControl.SetXpValue(_lastLevelContext.XpPoints);
            }
            
            _canvasService.TransitionController
                .PlayTransition<TransitionCrossFadeCanvasWindowLayerController,
                        GameplayCanvasWindow,
                        CompleteLevelCanvasWindow>
                    (true, false, null);
        }

        private void SetButtonsStatus(bool status)
        {
            _completeLevelLayer.ContinueButton.interactable = status;
            _completeLevelLayer.AdRewardButton.interactable = status;
            _failedLevelLayer.ReturnInMainMenuButton.interactable = status;
        }
    }
}