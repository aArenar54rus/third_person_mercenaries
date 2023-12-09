using Arenar.Options;
using Arenar.Services.Localization;
using Arenar.Services.PlayerInputService;
using UnityEngine.InputSystem;


namespace Arenar.Services.UI
{
    public class OptionsWindowController : CanvasWindowController
    {
        private OptionsWindow _optionsWindow;
        
        private OptionsButtonsLayer optionsButtonsLayer;

        private IOptionsController _optionsController;
        private ILocalizationService _localizationService;

        
        private OptionsWindowController(IOptionsController optionsController,
            ILocalizationService localizationService,
            IPlayerInputService playerInputService)
            : base(playerInputService)
        {
            _optionsController = optionsController;
            _localizationService = localizationService;
            _playerInputService = playerInputService;
        }

        
        public override void Initialize(ICanvasService canvasService)
        {
            base.Initialize(canvasService);

            _optionsWindow = _canvasService.GetWindow<OptionsWindow>();
            
            InitMainMenuOptionsLayer();
            
            _optionsWindow.OnShowEnd.AddListener(OnWindowShowEnd_SelectElements);
            _optionsWindow.OnHideBegin.AddListener(OnWindowHideBegin_DeselectElements);
        }

        protected override void OnWindowShowEnd_SelectElements()
        {
            optionsButtonsLayer.MusicButton.Select();
            if (_playerInputService.InputActionCollection is PlayerInput playerInput)
                playerInput.UI.Decline.performed += OnInputAction_Decline;
        }

        protected override void OnWindowHideBegin_DeselectElements()
        {
            if (_playerInputService.InputActionCollection is PlayerInput playerInput)
                playerInput.UI.Decline.performed -= OnInputAction_Decline;
        }

        private void OnInputAction_Decline(InputAction.CallbackContext context)
        {
            OnBackButtonClick();
        }

        private void InitMainMenuOptionsLayer()
        {
            optionsButtonsLayer = _optionsWindow.GetWindowLayer<OptionsButtonsLayer>();
            
            optionsButtonsLayer.BackButton.onClick.AddListener(OnBackButtonClick);
            
            optionsButtonsLayer.MusicButton.onClick.AddListener(OnMusicButtonClick);
            optionsButtonsLayer.SoundButton.onClick.AddListener(OnSoundButtonClick);
            
            optionsButtonsLayer.LanguageLastButton.onClick.AddListener(OnLastLanguageButtonClick);
            optionsButtonsLayer.LanguageNextButton.onClick.AddListener(OnNextLanguageButtonClick);
            optionsButtonsLayer.LanguageButton.onClick.AddListener(OnNextLanguageButtonClick);
        }

        private void OnBackButtonClick()
        {
            _canvasService.TransitionController
                .PlayTransition<TransitionCrossFadeCanvasWindowLayerController,
                        OptionsWindow,
                        MainMenuWindow>
                            (false, false, null);
        }
        
        private void OnMusicButtonClick()
        {
            
        }

        private void OnSoundButtonClick()
        {
            
        }

        private void OnLastLanguageButtonClick()
        {
            LastLanguage();
        }

        private void OnNextLanguageButtonClick()
        {
            SetNextLanguage();
        }
        
        public void SetNextLanguage()
        {
            int nextLanguage = _localizationService.LanguageIndex + 1;
            if (nextLanguage >= _localizationService.Languages.Length)
                nextLanguage = 0;

            _localizationService.CurrentLanguage = _localizationService.Languages[nextLanguage];
        }

        public void LastLanguage()
        {
            int lastLanguage = _localizationService.LanguageIndex - 1;
            if (lastLanguage < 0)
                lastLanguage = _localizationService.Languages.Length - 1;

            _localizationService.CurrentLanguage = _localizationService.Languages[lastLanguage];
        }
    }
}