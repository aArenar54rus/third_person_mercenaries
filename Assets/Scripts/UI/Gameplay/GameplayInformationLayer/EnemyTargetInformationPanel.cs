using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Arenar.UI
{
    public class EnemyTargetInformationPanel : MonoBehaviour
    {
        [SerializeField] private RectTransform _panelSelf;
        [SerializeField] private TMP_Text _enemyNameText;
        [SerializeField] private TMP_Text _enemyHealthText;
        [SerializeField] private Slider _enemyHealthSlider;
        [SerializeField] private TMP_Text _enemyDescriptionText;


        public void SetEnemy(string enemyName, string enemyDescription)
        {
            _panelSelf.gameObject.SetActive(true);
            _enemyNameText.text = enemyName;
            _enemyDescriptionText.text = enemyDescription;
        }

        public void UpdateEnemyHealth(int enemyHealth, int enemyHealthMax)
        {
            _enemyHealthSlider.maxValue = enemyHealthMax;
            _enemyHealthSlider.value = enemyHealth;
            _enemyHealthText.text = enemyHealth + " / " + enemyHealthMax;
        }

        public void UnsetEnemy()
        {
            _panelSelf.gameObject.SetActive(false);
        }
    }
}