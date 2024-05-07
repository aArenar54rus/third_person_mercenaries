using Arenar.Services.PlayerInputService;
using UnityEngine;
using YG;


namespace Arenar.Services.UI
{
    public class GameplayTouchInputWindowController : CanvasWindowController
    {
        public TouchControlLayer _playerTouchInputLayer;

        
        
        public GameplayTouchInputWindowController(IPlayerInputService playerInputService)
            : base(playerInputService)
        {
            
        }
        
        
        public override void Initialize(ICanvasService canvasService)
        {
            base.Initialize(canvasService);

            var gameplayWindow = _canvasService.GetWindow<GameplayCanvasWindow>();
            _playerTouchInputLayer = gameplayWindow.GetWindowLayer<TouchControlLayer>();
            _canvasService.GetWindow<GameplayCanvasWindow>().OnShowEnd.AddListener(OnWindowShowEnd_SelectElements);
        }

        protected override void OnWindowShowEnd_SelectElements()
        {
            _playerTouchInputLayer.SetLayerStatus(YandexGame.EnvironmentData.isMobile);
        }

        protected override void OnWindowHideBegin_DeselectElements()
        {
            return;
        }
    }
}