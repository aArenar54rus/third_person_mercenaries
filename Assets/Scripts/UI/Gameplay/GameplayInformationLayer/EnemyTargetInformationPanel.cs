using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Arenar.Services.UI
{
    public class EnemyTargetInformationPanel : MonoBehaviour
    {
        [SerializeField] private RectTransform _panelSelf;
        [SerializeField] private TMP_Text _enemyNameText;
        [SerializeField] private TMP_Text _enemyHealthText;
        [SerializeField] private Slider _enemyHealthSlider;
        [SerializeField] private Slider _enemyHealthRegressSlider;
        [SerializeField] private TMP_Text _enemyDescriptionText;

        private Tween _healthRegressTween;
        

        public void SetEnemy(string enemyName, string enemyDescription)
        {
            _panelSelf.gameObject.SetActive(true);
            _enemyNameText.text = enemyName;
            _enemyDescriptionText.text = enemyDescription;
        }

        public void UpdateEnemyHealth(int enemyHealth, int enemyHealthMax, bool immediately)
        {
            _enemyHealthSlider.maxValue = enemyHealthMax;
            _enemyHealthRegressSlider.maxValue = enemyHealthMax;
            _enemyHealthSlider.value = enemyHealth;
            _enemyHealthText.text = enemyHealth + " / " + enemyHealthMax;

            _healthRegressTween?.Kill(false);
            
            if (enemyHealth == 0)
            {
                UnsetEnemy();
                return;
            }

            if (immediately)
            {
                _enemyHealthRegressSlider.value = enemyHealth;
            }
            else
            {
                _healthRegressTween = DOVirtual.Float(_enemyHealthRegressSlider.value, enemyHealth, 0.5f,
                    (value) =>
                    {
                        _enemyHealthRegressSlider.value = value;
                    });
            }
        }

        public void UnsetEnemy()
        {
            _healthRegressTween?.Kill(false);
            _panelSelf.gameObject.SetActive(false);
        }
    }
}