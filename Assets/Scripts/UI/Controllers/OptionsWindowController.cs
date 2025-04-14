using Arenar.AudioSystem;
using Arenar.Options;
using Arenar.Services.Localization;
using Arenar.Services.PlayerInputService;
using UnityEngine.InputSystem;


namespace Arenar.Services.UI
{
    public class OptionsWindowController : CanvasWindowController
    {
        private OptionsWindow _optionsWindow;

        private IOptionsController _optionsController;
        private ILocalizationService _localizationService;
        private IAudioSystemManager _audioSystemManager;
        private IUiSoundManager _uiSoundManager;

        private OptionsButtonsLayer _optionsButtonsLayer;

        
        private OptionsWindowController(IOptionsController optionsController,
            IAudioSystemManager audioSystemManager,
            IUiSoundManager uiSoundManager,
            ILocalizationService localizationService,
            IPlayerInputService playerInputService)
            : base(playerInputService)
        {
            _optionsController = optionsController;
            _audioSystemManager = audioSystemManager;
            _localizationService = localizationService;
            base.playerInputService = playerInputService;
            _uiSoundManager = uiSoundManager;
        }

        
        public override void Initialize(ICanvasService canvasService)
        {
            base.Initialize(canvasService);

            _optionsWindow = base.canvasService.GetWindow<OptionsWindow>();
            _optionsButtonsLayer = _optionsWindow.GetWindowLayer<OptionsButtonsLayer>();
            
            InitMainMenuOptionsLayer();
            
            _optionsWindow.OnShowEnd.AddListener(OnWindowShowEnd_SelectElements);
            _optionsWindow.OnHideBegin.AddListener(OnWindowHideBegin_DeselectElements);
        }

        protected override void OnWindowShowEnd_SelectElements()
        {
            MusicOption musicOption = _optionsController.GetOption<MusicOption>();
            _optionsButtonsLayer.MusicLockIcon.gameObject.SetActive(musicOption.Volume == 0);
            
            SoundOption soundOption = _optionsController.GetOption<SoundOption>();
            _optionsButtonsLayer.SoundLockIcon.gameObject.SetActive(soundOption.Volume == 0);

            _optionsButtonsLayer.MusicButton.Select();
            if (playerInputService.InputActionCollection is PlayerInput playerInput)
                playerInput.UI.Decline.performed += OnInputAction_Decline;

            SetButtonsStatus(true);
        }

        protected override void OnWindowHideBegin_DeselectElements()
        {
            if (playerInputService.InputActionCollection is PlayerInput playerInput)
                playerInput.UI.Decline.performed -= OnInputAction_Decline;
            
            SetButtonsStatus(false);
        }

        private void OnInputAction_Decline(InputAction.CallbackContext context)
        {
            OnBackButtonClick();
        }

        private void InitMainMenuOptionsLayer()
        {
            _optionsButtonsLayer = _optionsWindow.GetWindowLayer<OptionsButtonsLayer>();
            
            _optionsButtonsLayer.BackButton.onClick.AddListener(OnBackButtonClick);
            
            _optionsButtonsLayer.MusicButton.onClick.AddListener(OnMusicButtonClick);
            _optionsButtonsLayer.SoundButton.onClick.AddListener(OnSoundButtonClick);
            
            _optionsButtonsLayer.LanguageLastButton.onClick.AddListener(OnLastLanguageButtonClick);
            _optionsButtonsLayer.LanguageNextButton.onClick.AddListener(OnNextLanguageButtonClick);
            _optionsButtonsLayer.LanguageButton.onClick.AddListener(OnNextLanguageButtonClick);
        }

        private void SetButtonsStatus(bool status)
        {
            _optionsButtonsLayer.BackButton.interactable = status;
            _optionsButtonsLayer.MusicButton.interactable = status;
            _optionsButtonsLayer.SoundButton.interactable = status;
            _optionsButtonsLayer.LanguageLastButton.interactable = status;
            _optionsButtonsLayer.LanguageNextButton.interactable = status;
            _optionsButtonsLayer.LanguageButton.interactable = status;
        }

        private void OnBackButtonClick()
        {
            _uiSoundManager.PlaySound(UiSoundType.StandartButtonClick);
            canvasService.TransitionController
                .PlayTransition<TransitionCrossFadeCanvasWindowLayerController,
                        OptionsWindow,
                        MainMenuWindow>
                            (false, false, null);
        }
        
        private void OnMusicButtonClick()
        {
            _uiSoundManager.PlaySound(UiSoundType.StandartButtonClick);
            MusicOption musicOption = _optionsController.GetOption<MusicOption>();
            bool status;
            if (musicOption.Volume > 0)
            {
                musicOption.Volume = 0;
                status = false;
            }
            else
            {
                musicOption.Volume = 1;
                status = true;
            }
            
            _audioSystemManager.SetVolume(AudioSystemType.Music, status, musicOption.Volume);
            _optionsButtonsLayer.MusicLockIcon.gameObject.SetActive(!status);
        }

        private void OnSoundButtonClick()
        {
            SoundOption soundOption = _optionsController.GetOption<SoundOption>();
            UiSoundOption uiSoundOption = _optionsController.GetOption<UiSoundOption>();
            bool status;
            if (soundOption.Volume > 0)
            {
                soundOption.Volume = 0;
                uiSoundOption.Volume = 0;
                status = false;
            }
            else
            {
                soundOption.Volume = 1;
                uiSoundOption.Volume = 1;
                status = true;
            }
            
            _audioSystemManager.SetVolume(AudioSystemType.Sound, status, soundOption.Volume);
            _audioSystemManager.SetVolume(AudioSystemType.UI, status, uiSoundOption.Volume);
            _optionsButtonsLayer.SoundLockIcon.gameObject.SetActive(!status);
            _uiSoundManager.PlaySound(UiSoundType.StandartButtonClick);
        }

        private void OnLastLanguageButtonClick()
        {
            _uiSoundManager.PlaySound(UiSoundType.StandartButtonClick);
            LastLanguage();
        }

        private void OnNextLanguageButtonClick()
        {
            _uiSoundManager.PlaySound(UiSoundType.StandartButtonClick);
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