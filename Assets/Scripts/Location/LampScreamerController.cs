using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Arenar
{
    public class LampScreamerController : MonoBehaviour
    {
        [SerializeField] private Light _pointLight;
        [SerializeField] private float _minTime = 0.1f;
        [SerializeField] private float _maxTime = 1.0f;

        private Tween _tween;


        private void Start()
        {
            _pointLight ??= gameObject.GetComponent<Light>();

            if (_pointLight == null)
                return;

            ChangeLampStatus();
        }

        private void ChangeLampStatus()
        {
            _tween = DOVirtual.DelayedCall(Random.Range(_minTime, _maxTime), () =>
                {
                    _pointLight.enabled = !_pointLight.enabled;
                })
                .OnComplete(ChangeLampStatus);
        }

        private void OnDestroy()
        {
            if (_tween != null)
                _tween?.Kill(false);
        }
    }
}