namespace Arenar.Services.UI
{
    public class OptionsWindowController : CanvasWindowController
    {
        private MainMenuWindow _mainMenuWindow;
        private OptionsWindow _optionsWindow;
        
        private OptionsButtonsLayer optionsButtonsLayer;


        public override void Initialize(ICanvasService canvasService)
        {
            base.Initialize(canvasService);
            
            _mainMenuWindow = _canvasService.GetWindow<MainMenuWindow>();
            _optionsWindow = _canvasService.GetWindow<OptionsWindow>();
            
            InitMainMenuOptionsLayer();
        }

        private void InitMainMenuOptionsLayer()
        {
            optionsButtonsLayer = _optionsWindow.GetWindowLayer<OptionsButtonsLayer>();
            
            optionsButtonsLayer.BackButton.onClick.AddListener(OnBackButtonClick);
            
            optionsButtonsLayer.MusicButton.onClick.AddListener(OnMusicButtonClick);
            optionsButtonsLayer.SoundButton.onClick.AddListener(OnSoundButtonClick);
            
            optionsButtonsLayer.LanguageLastButton.onClick.AddListener(OnLastLanguageButtonClick);
            optionsButtonsLayer.LanguageNextButton.onClick.AddListener(OnNextLanguageButtonClick);
        }

        private void OnBackButtonClick()
        {
            _optionsWindow.Hide(false);
            _mainMenuWindow.Show(false);
        }
        
        private void OnMusicButtonClick()
        {
            
        }

        private void OnSoundButtonClick()
        {
            
        }

        private void OnLastLanguageButtonClick()
        {
            
        }

        private void OnNextLanguageButtonClick()
        {
            
        }
    }
}