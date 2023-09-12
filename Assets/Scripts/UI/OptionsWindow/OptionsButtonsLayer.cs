using UnityEngine;
using UnityEngine.UI;


namespace Arenar.Services.UI
{
    public class OptionsButtonsLayer : CanvasWindowLayer
    {
        [SerializeField] private Button _backButton;
        
        [Space(5)]
        [SerializeField] private Button _soundButton;
        [SerializeField] private Image _soundIcon;
        [SerializeField] private Image _soundLockIcon;
        
        [Space(5)]
        [SerializeField] private Button _musicButton;
        [SerializeField] private Image _musicIcon;
        [SerializeField] private Image _musicLockIcon;
        
        [Space(5)]
        [SerializeField] private Image _countryIcon;
        [SerializeField] private Button _languageNextButton;
        [SerializeField] private Button _languageLastButton;
        
        
        public Button BackButton => _backButton;
        public Button SoundButton => _soundButton;
        public Image SoundIcon => _soundIcon;
        public Image SoundLockIcon => _soundLockIcon;
        public Button MusicButton => _musicButton;
        public Image MusicIcon => _musicIcon;
        public Image MusicLockIcon => _musicLockIcon;
        public Image CountryIcon => _countryIcon;
        public Button LanguageNextButton => _languageNextButton;
        public Button LanguageLastButton => _languageLastButton;
    }
}