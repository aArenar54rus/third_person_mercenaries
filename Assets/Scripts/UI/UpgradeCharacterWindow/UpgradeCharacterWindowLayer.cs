using TMPro;
using UnityEngine;
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
        private Button acceptButton;
        [SerializeField]
        private Button returnUpgradeButton;
        [SerializeField]
        private TMP_Text scoreText;
        
        [Space(10)]
        [SerializeField]
        private UpgradeParameterPanelVisual upgradeParameterPanelVisualPrefab;
        
        
        public RectTransform UpgradePanelsContainer => upgradePanelsContainer;
        public Button CloseButton => closeButton;
        public Button AcceptButton => acceptButton;
        public Button ReturnUpgradeButton => returnUpgradeButton;
        public TMP_Text ScoreText => scoreText;
        public UpgradeParameterPanelVisual UpgradeParameterPanelVisualPrefab => upgradeParameterPanelVisualPrefab;
    }
}