using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Arenar.Services.UI
{
    public class GameplayPlayerParametersWindowLayer : CanvasWindowLayer
    {
        [SerializeField]
        private TMP_Text _healthText;
        [SerializeField]
        private Slider _healthSlider;
        
        [Space(5)]
        [SerializeField]
        private Button openUpgradeSkillsMenuButton;
        [SerializeField]
        private TMP_Text upgradeSkillsCountText;
        

        public Button OpenUpgradeSkillsMenuButton => openUpgradeSkillsMenuButton;
        public TMP_Text UpgradeSkillsCountText => upgradeSkillsCountText;


        public void UpdatePlayerHealth(int health, int healthMax)
        {
            _healthText.text = health + "/" + healthMax;

            _healthSlider.maxValue = healthMax;
            _healthSlider.value = health;
        }
    }
}