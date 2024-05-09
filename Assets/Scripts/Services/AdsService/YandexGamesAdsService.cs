using Arenar.AudioSystem;
using Arenar.Options;
using DG.Tweening;
using UnityEngine;
using YG;
using Zenject;


namespace Arenar
{
    public class YandexGamesAdsService : MonoBehaviour
    {
        private IOptionsController _optionsController;
        private IAudioSystemManager _audioSystemManager;
        
        private bool _isSoundOn;
        private bool _isMusicOn;

        private Tween _checkerTween;
        
        
        public bool IsAdShownProcess { get; private set; }


        [Inject]
        public void Construct(IOptionsController optionsController, IAudioSystemManager audioSystemManager)
        {
            _optionsController = optionsController;
            _audioSystemManager = audioSystemManager;
            DontDestroyOnLoad(this);
        }

        public void ShowInterstitial()
        {
            _audioSystemManager.DisableAudio(false);
            YandexGame.FullscreenShow();
            StartCheckerTween();
            IsAdShownProcess = true;
        }

        public void ShowRewarded()
        {
            _audioSystemManager.DisableAudio(false);
            YandexGame.RewVideoShow(0);
            StartCheckerTween();
            IsAdShownProcess = true;
        }

        private void StartCheckerTween()
        {
            _checkerTween = DOVirtual.DelayedCall(0.1f, () =>
            {
                if (YandexGame.nowAdsShow)
                    return;

                _audioSystemManager.EnableAudio();
                IsAdShownProcess = false;
                _checkerTween?.Kill();
            }).SetLoops(-1);
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                _audioSystemManager.DisableAudio(false);
            }
            else
            {
                if (!IsAdShownProcess)
                    _audioSystemManager.EnableAudio();
            }
        }
    }
}