using Arenar.Services.PlayerInputService;
using Arenar.Services.SaveAndLoad;


namespace Arenar.Services.UI
{
    public class MainMenuWindowController : CanvasWindowController
    {
        private ISaveAndLoadService<SaveDelegate> _saveAndLoadService;
        private IPlayerInputService _playerInputService;
        
        private MainMenuWindow _mainMenuWindow;
        private MainMenuButtonsLayer _mainMenuButtonsLayer;
        private MainMenuOptionsLayer _mainMenuOptionsLayer;
        private MainMenuPlayerInformationLayer _mainMenuPlayerInfoLayer;

        private LevelSelectionWindow _levelSelectionWindow;
        private InventoryCanvasWindow _inventoryCanvasWindow;


        public MainMenuWindowController(ISaveAndLoadService<SaveDelegate> saveAndLoadService, IPlayerInputService playerInputService)
        {
            _saveAndLoadService = saveAndLoadService;
            _playerInputService = playerInputService;
        }
        

        public override void Initialize(ICanvasService canvasService)
        {
            base.Initialize(canvasService);

            _mainMenuWindow = _canvasService.GetWindow<MainMenuWindow>();
            _levelSelectionWindow = _canvasService.GetWindow<LevelSelectionWindow>();
            _inventoryCanvasWindow = _canvasService.GetWindow<InventoryCanvasWindow>();

            InitMainMenuButtonsLayer();
            InitMainMenuOptionsLayer();
            InitMainMenuPlayerInformationLayer();
            
            _playerInputService.SetNewInputControlType(InputActionMapType.UI, true);
        }

        private void InitMainMenuButtonsLayer()
        {
            _mainMenuButtonsLayer = _mainMenuWindow.GetWindowLayer<MainMenuButtonsLayer>();
            
            _mainMenuButtonsLayer.NewChallengeButton.onClick.AddListener(OnNewChallengeButtonClick);
            _mainMenuButtonsLayer.OutfitButton.onClick.AddListener(OnOutfitButtonClick);
            _mainMenuButtonsLayer.OptionsButton.onClick.AddListener(OnOptionsButtonClick);
            _mainMenuButtonsLayer.RateUsButton.onClick.AddListener(OnRateUsButtonClick);
        }

        private void InitMainMenuOptionsLayer()
        {
            _mainMenuOptionsLayer = _mainMenuWindow.GetWindowLayer<MainMenuOptionsLayer>();
            
            _mainMenuOptionsLayer.MusicButton.onClick.AddListener(OnMusicButtonClick);
            _mainMenuOptionsLayer.SoundButton.onClick.AddListener(OnSoundButtonClick);
            
            _mainMenuOptionsLayer.LanguageLastButton.onClick.AddListener(OnSoundButtonClick);
            _mainMenuOptionsLayer.LanguageNextButton.onClick.AddListener(OnSoundButtonClick);
        }

        private void InitMainMenuPlayerInformationLayer()
        {
            _mainMenuPlayerInfoLayer = _mainMenuWindow.GetWindowLayer<MainMenuPlayerInformationLayer>();

            _mainMenuPlayerInfoLayer.NickNameText.text = "Player";
        }

        private void OnNewChallengeButtonClick()
        {
            // _mainMenuButtonsLayer.HideWindowLayer(false);
            _mainMenuWindow.Hide(false);
            _levelSelectionWindow.Show(false);
        }

        private void OnOutfitButtonClick()
        {
            _mainMenuWindow.Hide(false);
            _inventoryCanvasWindow.Show(false);
        }

        private void OnOptionsButtonClick()
        { 
            _mainMenuButtonsLayer.HideWindowLayer(false);
            _mainMenuOptionsLayer.ShowWindowLayer(false);
        }

        private void OnRateUsButtonClick()
        {
            // web rate us
        }

        private void OnMusicButtonClick()
        {
            
        }

        private void OnSoundButtonClick()
        {
            
        }
    }
}