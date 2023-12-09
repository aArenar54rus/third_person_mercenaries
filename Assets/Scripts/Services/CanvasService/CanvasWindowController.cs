using System;
using Arenar.Services.PlayerInputService;
using Zenject;


namespace Arenar.Services.UI
{
    public abstract class CanvasWindowController
	{
		protected ICanvasService _canvasService;
		protected IPlayerInputService _playerInputService;
		
		
		public CanvasWindowController(IPlayerInputService playerInputService) =>
			_playerInputService = playerInputService;
		

		public virtual void Initialize(ICanvasService canvasService) =>
			_canvasService = canvasService;

		protected abstract void OnWindowShowEnd_SelectElements();
		protected abstract void OnWindowHideBegin_DeselectElements();
		
		
		public class Factory : PlaceholderFactory<Type, CanvasWindowController> {}
    }
}
