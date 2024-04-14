using DG.Tweening;
using UnityEngine;

public class AutoDisableVfxEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particle;
    [SerializeField] private float _activeSecondsMax;

    private float _activeSeconds = 0;

    private void OnEnable()
    {
        _activeSeconds = 0;
    }

    private void Update()
    {
        _activeSeconds += Time.deltaTime;
        if (_activeSeconds >= _activeSecondsMax)
        {
            _particle.Stop();
            gameObject.SetActive(false);
        }
    }
}
