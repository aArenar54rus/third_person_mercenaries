using System;
using Zenject;


namespace Arenar.Services.UI
{
    public abstract class CanvasWindowController
	{
		protected ICanvasService _canvasService;


		public virtual void Initialize(ICanvasService canvasService) =>
			_canvasService = canvasService;
		

		public class Factory : PlaceholderFactory<Type, CanvasWindowController> {}
    }
}
