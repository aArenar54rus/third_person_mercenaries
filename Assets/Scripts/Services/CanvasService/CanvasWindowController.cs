using System;
using Arenar.Services.PlayerInputService;
using Zenject;


namespace Arenar.Services.UI
{
	public abstract class CanvasWindowController
	{
		protected ICanvasService canvasService;
		protected IPlayerInputService playerInputService;
		
		
		public CanvasWindowController(IPlayerInputService playerInputService) =>
			this.playerInputService = playerInputService;
		

		public virtual void Initialize(ICanvasService canvasService) =>
			this.canvasService = canvasService;

		protected abstract void OnWindowShowEnd_SelectElements();
		protected abstract void OnWindowHideBegin_DeselectElements();
		
		
		public class Factory : PlaceholderFactory<Type, CanvasWindowController> {}
	}
}
