using Arenar.AudioSystem;
using Arenar.Options;
using DG.Tweening;
using YG;
using Zenject;


namespace Arenar
{
    public class YandexGamesAdsService
    {
        private IOptionsController _optionsController;
        private IAudioSystemManager _audioSystemManager;
        
        private bool _isSoundOn;
        private bool _isMusicOn;

        private Tween _checkerTween;


        [Inject]
        public void Construct(IOptionsController optionsController, IAudioSystemManager audioSystemManager)
        {
            _optionsController = optionsController;
            _audioSystemManager = audioSystemManager;
        }

        public void ShowInterstitial()
        {
            DisableAudio();
            YandexGame.FullscreenShow();
            StartCheckerTween();
        }

        public void ShowRewarded()
        {
            DisableAudio();
            YandexGame.RewVideoShow(0);
            StartCheckerTween();
        }

        private void StartCheckerTween()
        {
            _checkerTween = DOVirtual.DelayedCall(0.1f, () =>
            {
                if (YandexGame.nowAdsShow)
                    return;

                EnableAudio();
                    
                _checkerTween?.Kill();
            }).SetLoops(-1);
        }

        private void DisableAudio()
        {
            _isMusicOn = _optionsController.GetOption<MusicOption>().IsActive;
            if (_isMusicOn)
            {
                _audioSystemManager.SetVolume(AudioSystemType.Music, false, 0);
            }

            _isSoundOn = _optionsController.GetOption<SoundOption>().IsActive;
            if (_isSoundOn)
            {
                _audioSystemManager.SetVolume(AudioSystemType.Sound, false, 0);
                _audioSystemManager.SetVolume(AudioSystemType.UI, false, 0);
            }
        }

        private void EnableAudio()
        {
            if (_isMusicOn)
                _audioSystemManager.SetVolume(AudioSystemType.Music, _isMusicOn, 1);
                    
            if (!_isSoundOn)
            {
                _audioSystemManager.SetVolume(AudioSystemType.Sound, _isSoundOn, 1);
                _audioSystemManager.SetVolume(AudioSystemType.UI, _isSoundOn, 1);
            }
        }
    }
}