using System;
using Zenject;


namespace Arenar.Services.UI
{
    public abstract class CanvasWindowController
	{
		protected ICanvasService canvasService;


		public virtual void Initialize(ICanvasService canvasService) =>
			this.canvasService = canvasService;
		

		public class Factory : PlaceholderFactory<Type, CanvasWindowController> {}
    }
}
