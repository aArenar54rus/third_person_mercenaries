using Arenar.AudioSystem;
using Arenar.Character;
using Arenar.Services.PlayerInputService;
using Arenar.Services.SaveAndLoad;
using Arenar.SimpleYandexGames;
using I2.Loc;
using TakeTop.PreferenceSystem;


namespace Arenar.Services.UI
{
    public class MainMenuWindowController : CanvasWindowController
    {
        private IPreferenceManager _preferenceManager;

        private MainMenuWindow _mainMenuWindow;
        
        private MainMenuButtonsLayer _mainMenuButtonsLayer;
        private MainMenuPlayerInformationLayer _mainMenuPlayerInfoLayer;
        private PlayerCharacterLevelData _playerCharacterLevelData;
        
        private OptionsWindow _optionsWindow;
        private IAmbientManager _ambientManager;
        private IUiSoundManager _uiSoundManager;

        private YandexGames _yandexGames;


        public MainMenuWindowController(IPreferenceManager preferenceManager,
            PlayerCharacterLevelData playerCharacterLevelData,
            IPlayerInputService playerInputService,
            IAmbientManager ambientManager,
            IUiSoundManager uiSoundManager/*,
            YandexGames yandexGames*/)
            : base(playerInputService)
        {
            _preferenceManager = preferenceManager;
            _playerCharacterLevelData = playerCharacterLevelData;
            _playerInputService = playerInputService;
            _ambientManager = ambientManager;
            _uiSoundManager = uiSoundManager;
            //_yandexGames = yandexGames;
        }
        

        public override void Initialize(ICanvasService canvasService)
        {
            base.Initialize(canvasService);

            _mainMenuWindow = _canvasService.GetWindow<MainMenuWindow>();
            _optionsWindow = _canvasService.GetWindow<OptionsWindow>();

            InitMainMenuButtonsLayer();
            InitMainMenuPlayerInformationLayer();
            
            _playerInputService.SetInputControlType(InputActionMapType.UI, true);
            
            _mainMenuWindow.OnHideBegin.AddListener(OnWindowHideBegin_DeselectElements);

            _mainMenuWindow.OnShowBegin.AddListener(OnShowWindowBegin);
            _mainMenuWindow.OnShowEnd.AddListener(OnWindowShowEnd_SelectElements);
        }

        private void InitMainMenuButtonsLayer()
        {
            _mainMenuButtonsLayer = _mainMenuWindow.GetWindowLayer<MainMenuButtonsLayer>();
            
            _mainMenuButtonsLayer.NewChallengeButton.onClick.AddListener(OnNewChallengeButtonClick);
            _mainMenuButtonsLayer.OutfitButton.onClick.AddListener(OnOutfitButtonClick);
            _mainMenuButtonsLayer.OptionsButton.onClick.AddListener(OnOptionsButtonClick);
            _mainMenuButtonsLayer.RateUsButton.onClick.AddListener(OnRateUsButtonClick);
        }

        private void InitMainMenuPlayerInformationLayer()
        {
            _mainMenuPlayerInfoLayer = _mainMenuWindow.GetWindowLayer<MainMenuPlayerInformationLayer>();

            _mainMenuPlayerInfoLayer.NickNameText.text = "Player";
        }

        private void OnNewChallengeButtonClick()
        {
            _uiSoundManager.PlaySound(UiSoundType.StandartButtonClick);
            _canvasService.TransitionController
                .PlayTransition<TransitionCrossFadeCanvasWindowLayerController,
                                MainMenuWindow,
                                LevelSelectionWindow>
                                    (false, false, null);
        }

        private void OnOutfitButtonClick()
        {
            _uiSoundManager.PlaySound(UiSoundType.StandartButtonClick);
            _canvasService.TransitionController
                .PlayTransition<TransitionCrossFadeCanvasWindowLayerController,
                        MainMenuWindow,
                        InventoryCanvasWindow>
                    (false, false, null);
        }

        private void OnOptionsButtonClick()
        {
            _uiSoundManager.PlaySound(UiSoundType.StandartButtonClick);
            _canvasService.TransitionController
                .PlayTransition<TransitionCrossFadeCanvasWindowLayerController,
                        MainMenuWindow,
                        OptionsWindow>
                    (false, false, null);
        }

        private void OnRateUsButtonClick()
        {
            _uiSoundManager.PlaySound(UiSoundType.StandartButtonClick);
            //_yandexGames.
            // web rate us
        }

        protected override void OnWindowShowEnd_SelectElements()
        {
            _mainMenuButtonsLayer.NewChallengeButton.Select();
            SetButtonsStatus(true);
        }

        protected override void OnWindowHideBegin_DeselectElements()
        {
            SetButtonsStatus(false);
        }

        private void OnShowWindowBegin()
        {
            UpdatePlayerData();
            _ambientManager.PlayAmbient(AmbientType.MainMenu, true);
        }
        
        private void UpdatePlayerData()
        {
            var playerProgress = _preferenceManager.LoadValue<PlayerSaveDelegate>();
            _mainMenuPlayerInfoLayer = _mainMenuWindow.GetWindowLayer<MainMenuPlayerInformationLayer>();

            if (playerProgress.playerCharacterLevel >= _playerCharacterLevelData.MaxLevel)
            {
                string maxLevelText = LocalizationManager.GetTranslation("loc_key_max_level");
                _mainMenuPlayerInfoLayer.LevelText.text = maxLevelText;
                _mainMenuPlayerInfoLayer.LevelProgressSlider.maxValue = 1;
                _mainMenuPlayerInfoLayer.LevelProgressSlider.value = 1;
            }
            else
            {
                _mainMenuPlayerInfoLayer.LevelText.text = (playerProgress.playerCharacterLevel + 1).ToString();
                _mainMenuPlayerInfoLayer.LevelProgressSlider.maxValue =
                    _playerCharacterLevelData.GetExperienceForNextLevel(playerProgress.playerCharacterLevel);
                _mainMenuPlayerInfoLayer.LevelProgressSlider.value = playerProgress.currentXpPoints;
            }
        }

        private void SetButtonsStatus(bool status)
        {
            _mainMenuButtonsLayer.NewChallengeButton.interactable = status;
            _mainMenuButtonsLayer.OutfitButton.interactable = status;
            _mainMenuButtonsLayer.OptionsButton.interactable = status;
            _mainMenuButtonsLayer.RateUsButton.interactable = status;
        }
    }
}