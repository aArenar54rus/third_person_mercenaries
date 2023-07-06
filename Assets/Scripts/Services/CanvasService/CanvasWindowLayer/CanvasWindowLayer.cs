using System;
using UnityEngine;


namespace Arenar.Services.UI
{
    public abstract class CanvasWindowLayer : MonoBehaviour
    {
		private Canvas canvas = default;
		public Action<CanvasWindowLayer> onCanvasLayerShowBegin;
		public Action<CanvasWindowLayer> onCanvasLayerShowEnd;
		public Action<CanvasWindowLayer> onCanvasLayerHideBegin;
		public Action<CanvasWindowLayer> onCanvasLayerHideEnd;


		protected Canvas Canvas
        {
            get
            {
                if (canvas != null)
                    return canvas;

                if (gameObject.TryGetComponent(out Canvas objectCanvas))
                {
                    canvas = objectCanvas;
                    return canvas;
                }

                canvas = gameObject.AddComponent<Canvas>();
                return canvas;
            }
        }


		public virtual void Initialize() {}

		public virtual void DeInitialize() {}

		public virtual void ShowWindowLayer(bool isImmediately = true)
		{
			Canvas.enabled = true;
			OnCanvasWindowShowBegin();
			OnCanvasWindowShowEnd();
		}

		public virtual void HideWindowLayer(bool isImmediately = true)
		{
			OnCanvasWindowHideBegin();
			OnCanvasWindowHideEnd();
			Canvas.enabled = false;
		}

		protected virtual void OnCanvasWindowShowBegin()
		{
			onCanvasLayerShowBegin?.Invoke(this);
		}

		protected virtual void OnCanvasWindowShowEnd()
		{
			onCanvasLayerShowEnd?.Invoke(this);
		}

		protected virtual void OnCanvasWindowHideBegin()
		{
			onCanvasLayerHideBegin?.Invoke(this);
		}

		protected virtual void OnCanvasWindowHideEnd()
		{
			onCanvasLayerHideEnd?.Invoke(this);
		}
	}
}
