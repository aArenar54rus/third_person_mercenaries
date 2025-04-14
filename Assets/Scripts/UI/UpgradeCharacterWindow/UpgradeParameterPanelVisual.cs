using Arenar.Character;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Arenar.Services.UI
{
    public class UpgradeParameterPanelVisual : MonoBehaviour
    {
        [SerializeField]
        private Button upgradeButton;
        [SerializeField]
        private Image upgradeIcon;
        [SerializeField]
        private TMP_Text upgradeNameText;
        [SerializeField]
        private TMP_Text levelText;
        [SerializeField]
        private TMP_Text addedParameterText;
        
        [Space(10)]
        [SerializeField]
        private Color defaultTextColor = Color.white;
        [SerializeField]
        private Color upgradedTextColor = Color.green;


        public Button UpgradeButton => upgradeButton;
        

        public void InitializeUpgradeParameter(CharacterSkillUpgradeType type)
        {
            upgradeNameText.text = type.ToString();
        }
        
        public void SetUpgradeProgress(
            int level,
            bool isMaxLevel,
            bool isUpgraded,
            float progress
        )
        {
            levelText.text = "LVL: " + (isMaxLevel ? "MAX" : (level + 1).ToString());
            levelText.color = isUpgraded ? upgradedTextColor : defaultTextColor;
            
            addedParameterText.text = "ADDED: " + progress.ToString();
            addedParameterText.color = isUpgraded ? upgradedTextColor : defaultTextColor;
        }
        
        public void SetButtonInteractable(bool status)
        {
            upgradeButton.interactable = status;
        }
    }
}