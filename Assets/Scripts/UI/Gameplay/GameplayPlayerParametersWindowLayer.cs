using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Arenar.Services.UI
{
    public class GameplayPlayerParametersWindowLayer : CanvasWindowLayer
    {
        [SerializeField] private TMP_Text _healthText;
        [SerializeField] private Slider _healthSlider;
        
        [Space(5)]
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _experienceText;
        [SerializeField] private Slider _experienceSlider;


        public void UpdatePlayerHealth(int health, int healthMax)
        {
            _healthText.text = health + "/" + healthMax;

            _healthSlider.maxValue = healthMax;
            _healthSlider.value = health;
        }

        public void UpdatePlayerExperience(int experience, int experienceMax)
        {
            _experienceText.text = experience + "/" + experienceMax;
            
            _experienceSlider.maxValue = experienceMax;
            _experienceSlider.value = experience;
        }

        public void UpdatePlayerLevel(int level) =>
            _levelText.text = level.ToString();
    }
}