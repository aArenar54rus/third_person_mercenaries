using Arenar.Services.LevelsService;
using Arenar.Services.PlayerInputService;
using UnityEngine.InputSystem;


namespace Arenar.Services.UI
{
    public class CompleteLevelWindowController : CanvasWindowController
    {
        private ILevelsService _levelsService;
        
        private CompleteLevelCanvasWindow _completeLevelCanvasWindow;
        private CompleteLevelLayer _completeLevelLayer;


        private ProgressLevelMarksVisualControl ProgressLevelMarksVisualControl =>
            _completeLevelLayer.ProgressLevelMarksVisualControl;

        private CompleteLevelItemRewardsVisualControl CompleteLevelItemRewardsVisualControl =>
            _completeLevelLayer.CompleteLevelItemRewardsVisualControl;

        
        public CompleteLevelWindowController(ILevelsService levelsService, IPlayerInputService playerInputService) : base(playerInputService)
        {
            _playerInputService = playerInputService;
            _levelsService = levelsService;
        }


        public override void Initialize(ICanvasService canvasService)
        {
            base.Initialize(canvasService);
            
            _completeLevelCanvasWindow = _canvasService.GetWindow<CompleteLevelCanvasWindow>();
            _completeLevelLayer = _completeLevelCanvasWindow.GetWindowLayer<CompleteLevelLayer>();
            
            _completeLevelLayer.ContinueButton.onClick.AddListener(OnContinueButtonClick);

            _levelsService.onCompleteLevel += OnLevelComplete;
            _completeLevelCanvasWindow.OnShowEnd.AddListener(OnWindowShowEnd_SelectElements);
            _completeLevelCanvasWindow.OnHideBegin.AddListener(OnWindowHideBegin_DeselectElements);
        }

        protected override void OnWindowShowEnd_SelectElements()
        {
            _completeLevelLayer.ContinueButton.Select();
            if (_playerInputService.InputActionCollection is PlayerInput playerInput)
                playerInput.UI.Decline.performed += OnInputAction_Decline;
        }

        protected override void OnWindowHideBegin_DeselectElements()
        {
            if (_playerInputService.InputActionCollection is PlayerInput playerInput)
                playerInput.UI.Decline.performed -= OnInputAction_Decline;
        }

        private void OnInputAction_Decline(InputAction.CallbackContext callbackContext) =>
            OnContinueButtonClick();

        private void OnContinueButtonClick()
        {
            
        }

        private void OnLevelComplete(LevelContext levelContext)
        {
            
        }
    }
}