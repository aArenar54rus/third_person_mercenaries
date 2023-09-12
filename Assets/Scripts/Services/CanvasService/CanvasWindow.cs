using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using Zenject;


namespace Arenar.Services.UI
{
    public abstract class CanvasWindow : MonoBehaviour
    {
	    [SerializeField] protected CanvasWindowLayer[] canvasWindowLayers = default;

	    private Canvas canvas;
		private List<CanvasWindowLayer> activeWindowLayers;
		private Tween animationTween;


		public UnityEvent OnShowBegin { get; } = new();
        public UnityEvent OnShowEnd { get; } = new();
        public UnityEvent OnHideBegin { get; } = new();
        public UnityEvent OnHideEnd { get; } = new();


		public virtual bool OverrideSorting => false;

        public Canvas Canvas
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


        public virtual void Show(bool immediately = false, Action OnShowEndCallback = null)
        {
			if (OnShowEndCallback != null)
				OnShowEnd.AddListener(OnShowEndCallback.Invoke);

            gameObject.SetActive(true);
            OnShowBegin?.Invoke();
            OnShowBegin?.RemoveAllListeners();

            foreach (CanvasWindowLayer canvasWindowElement in canvasWindowLayers)
	            canvasWindowElement.ShowWindowLayer(immediately);
		}

        public virtual void Hide(bool immediately = false, Action onHideEndCallback = null)
        {
			if (onHideEndCallback != null)
				OnHideEnd.AddListener(onHideEndCallback.Invoke);

            OnHideBegin?.Invoke();
            OnHideBegin?.RemoveAllListeners();

            foreach (CanvasWindowLayer canvasWindowElement in canvasWindowLayers)
                canvasWindowElement.HideWindowLayer(immediately);
		}

		public T GetWindowLayer<T>() where T : CanvasWindowLayer
        {
            foreach (var canvasWindowElement in canvasWindowLayers)
            {
                if (canvasWindowElement is T)
                    return (T)canvasWindowElement;
            }

            Debug.LogError($"Not found {typeof(T)}.");
            return null;
        }

		public virtual void Initialize()
		{
			activeWindowLayers = new List<CanvasWindowLayer>();
			canvasWindowLayers[0].onCanvasLayerShowEnd += OnWindowShowEnd;

			foreach (CanvasWindowLayer canvasWindowLayer in canvasWindowLayers)
			{
				canvasWindowLayer.Initialize();
				canvasWindowLayer.onCanvasLayerShowBegin += OnWindowShowBegin;
				canvasWindowLayer.onCanvasLayerHideEnd += OnWindowHideEnd;
			}
		}

		protected virtual void DeInitialize()
		{
			canvasWindowLayers[0].onCanvasLayerShowEnd -= OnWindowShowEnd;
			foreach (CanvasWindowLayer canvasWindowLayer in canvasWindowLayers)
			{
				canvasWindowLayer.DeInitialize();
				canvasWindowLayer.onCanvasLayerShowBegin -= OnWindowShowBegin;
				canvasWindowLayer.onCanvasLayerHideEnd -= OnWindowHideEnd;
			}
		}

		private void OnShowComplete()
		{
			OnShowEnd?.Invoke();
		}

		private void OnHideComplete()
		{
			OnHideEnd?.Invoke();
			gameObject.SetActive(false);
		}

		private void OnWindowShowBegin(CanvasWindowLayer canvasWindowLayer)
		{
			if (!canvasWindowLayers.Contains(canvasWindowLayer))
			{
				Debug.LogError($"Can't find {canvasWindowLayer.name} on canvas window. Check actions.");
				return;
			}

			if (activeWindowLayers.Contains(canvasWindowLayer))
				return;

			activeWindowLayers.Add(canvasWindowLayer);
		}

		private void OnWindowShowEnd(CanvasWindowLayer canvasWindowLayer)
		{
			if (!canvasWindowLayer != default)
			{
				Debug.LogError($"Can't find {canvasWindowLayer.name} on default canvas window. Check actions.");
				return;
			}

			OnShowComplete();
		}

		private void OnWindowHideEnd(CanvasWindowLayer canvasWindowLayer)
		{
			if (!canvasWindowLayers.Contains(canvasWindowLayer))
			{
				Debug.LogError($"Can't find {canvasWindowLayer.name} on canvas window. Check actions.");
				return;
			}

			if (!activeWindowLayers.Contains(canvasWindowLayer))
				return;

			activeWindowLayers.Remove(canvasWindowLayer);

			if (activeWindowLayers.Count == 0)
				OnHideComplete();
		}

		private void OnDestroy() =>
			DeInitialize();


		public class Factory : PlaceholderFactory<UnityEngine.Object, Transform, CanvasWindow> {}
    }
}
