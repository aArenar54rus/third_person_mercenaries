using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Arenar.Services.UI
{
    public class FailedLevelLayer : CanvasWindowLayer
    {
        [SerializeField] private CompleteLevelItemRewardsVisualControl _completeLevelItemRewardsVisualControl;
        [SerializeField] private TMP_Text _rewardText;
        [SerializeField] private Button _returnInMainMenuButton;


        public CompleteLevelItemRewardsVisualControl CompleteLevelItemRewardsVisualControl =>
            _completeLevelItemRewardsVisualControl;
        
        public TMP_Text RewardText => _rewardText;
        
        public Button ReturnInMainMenuButton => _returnInMainMenuButton;
    }
}