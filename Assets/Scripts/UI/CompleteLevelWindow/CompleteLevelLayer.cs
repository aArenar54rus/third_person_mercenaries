using UnityEngine;
using UnityEngine.UI;


namespace Arenar.Services.UI
{
    public class CompleteLevelLayer : CanvasWindowLayer
    {
        [SerializeField] private ProgressLevelMarksVisualControl _progressLevelMarksVisualControl;
        [SerializeField] private CompleteLevelItemRewardsVisualControl _completeLevelItemRewardsVisualControl;
        [SerializeField] private Button _continueButton;


        public ProgressLevelMarksVisualControl ProgressLevelMarksVisualControl =>
            _progressLevelMarksVisualControl;
        
        public CompleteLevelItemRewardsVisualControl CompleteLevelItemRewardsVisualControl =>
            _completeLevelItemRewardsVisualControl;
        
        public Button ContinueButton => _continueButton;
    }
}