using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Arenar.Services.UI
{
    public class UpgradeCharacterWindowLayer : CanvasWindowLayer
    {
        [SerializeField]
        private RectTransform upgradePanelsContainer;
        [SerializeField]
        private Button closeButton;
        [SerializeField]
        private Button returnUpgradeButton;
        
        [Space(10)]
        [SerializeField]
        private UpgradeParameterPanelVisual upgradeParameterPanelVisualPrefab;
        
        
        public RectTransform UpgradePanelsContainer => upgradePanelsContainer;
        public Button CloseButton => closeButton;
        public Button ReturnUpgradeButton => returnUpgradeButton;
        public UpgradeParameterPanelVisual UpgradeParameterPanelVisualPrefab => upgradeParameterPanelVisualPrefab;
    }
}