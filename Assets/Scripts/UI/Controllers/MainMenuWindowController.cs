using Arenar.Services.PlayerInputService;
using Arenar.Services.SaveAndLoad;


namespace Arenar.Services.UI
{
    public class MainMenuWindowController : CanvasWindowController
    {
        private ISaveAndLoadService<SaveDelegate> _saveAndLoadService;

        private MainMenuWindow _mainMenuWindow;
        private OptionsWindow _optionsWindow;
        
        private MainMenuButtonsLayer _mainMenuButtonsLayer;
        private MainMenuPlayerInformationLayer _mainMenuPlayerInfoLayer;


        public MainMenuWindowController(ISaveAndLoadService<SaveDelegate> saveAndLoadService, IPlayerInputService playerInputService)
            : base(playerInputService)
        {
            _saveAndLoadService = saveAndLoadService;
            _playerInputService = playerInputService;
        }
        

        public override void Initialize(ICanvasService canvasService)
        {
            base.Initialize(canvasService);

            _mainMenuWindow = _canvasService.GetWindow<MainMenuWindow>();
            _optionsWindow = _canvasService.GetWindow<OptionsWindow>();

            InitMainMenuButtonsLayer();
            InitMainMenuPlayerInformationLayer();
            
            _playerInputService.SetInputControlType(InputActionMapType.UI, true);

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
            _canvasService.TransitionController
                .PlayTransition<TransitionCrossFadeCanvasWindowLayerController,
                                MainMenuWindow,
                                LevelSelectionWindow>
                                    (false, false, null);
        }

        private void OnOutfitButtonClick()
        {
            _canvasService.TransitionController
                .PlayTransition<TransitionCrossFadeCanvasWindowLayerController,
                        MainMenuWindow,
                        InventoryCanvasWindow>
                    (false, false, null);
        }

        private void OnOptionsButtonClick()
        {
            _canvasService.TransitionController
                .PlayTransition<TransitionCrossFadeCanvasWindowLayerController,
                        MainMenuWindow,
                        OptionsWindow>
                    (false, false, null);
        }

        private void OnRateUsButtonClick()
        {
            // web rate us
        }

        protected override void OnWindowShowEnd_SelectElements()
        {
            _mainMenuButtonsLayer.NewChallengeButton.Select();
        }

        protected override void OnWindowHideBegin_DeselectElements() { }
    }
}