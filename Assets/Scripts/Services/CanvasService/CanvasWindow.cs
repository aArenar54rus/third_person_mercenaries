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
	    
	    [Space(10)]
	    [SerializeField] private CanvasWindowsLayerAnimationType _animationType;
	    [SerializeField] private Animator _windowAnimator;
	    [SerializeField] private string _showAnimationName = default;
	    [SerializeField] private string _idleAnimationName = default;
	    [SerializeField] private string _hideAnimationName = default;
	    [SerializeField] private string _hiddenAnimationName = default;
	    
	    protected Tween _windowAnimationTween;

	    private Canvas canvas;
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

                if (gameObject.TryGetComponent<Canvas>(out canvas))
	                return canvas;
                
                canvas = gameObject.AddComponent<Canvas>();
                return canvas;
            }
        }
        
        
        
        public virtual void Initialize()
        {
	        foreach (CanvasWindowLayer canvasWindowLayer in canvasWindowLayers)
		        canvasWindowLayer.Initialize();
        }
        
        public virtual void Show(bool immediately = false, Action OnShowEndCallback = null)
        {
			if (OnShowEndCallback != null)
				OnShowEnd.AddListener(OnShowEndCallback.Invoke);

            gameObject.SetActive(true);
            Canvas.enabled = true;
            Debug.LogError(gameObject.name + " open " + gameObject.activeSelf);
            
            foreach (CanvasWindowLayer canvasWindowLayer in canvasWindowLayers)
	            canvasWindowLayer.ShowWindowLayerStart();

            switch (_animationType)
            {
	            case CanvasWindowsLayerAnimationType.None:
		            OnShowComplete();
		            break;
				
	            case CanvasWindowsLayerAnimationType.Tween:
		            OnTweenAnimationShowPlay(immediately);
		            break;
				
	            case CanvasWindowsLayerAnimationType.ClassicUnity:
		            OnUnityAnimatorShowPlay(immediately);
		            break;
				
	            default:
		            Debug.LogError("Unknown window animation type: " + _animationType);
		            OnShowComplete();
		            break;
            }
            
            OnShowBegin?.Invoke();
            OnShowBegin?.RemoveAllListeners();

            Debug.LogError(gameObject.name + " open " + gameObject.activeSelf);
        }

        public virtual void Hide(bool immediately = false, Action onHideEndCallback = null)
        {
			if (onHideEndCallback != null)
				OnHideEnd.AddListener(onHideEndCallback.Invoke);
			
			foreach (CanvasWindowLayer canvasWindowElement in canvasWindowLayers)
				canvasWindowElement.HideWindowLayerStart();

			switch (_animationType)
            {
	            case CanvasWindowsLayerAnimationType.None:
		            OnHideComplete();
		            break;
				
	            case CanvasWindowsLayerAnimationType.Tween:
		            OnTweenAnimationHidePlay(immediately);
		            break;
				
	            case CanvasWindowsLayerAnimationType.ClassicUnity:
		            OnUnityAnimatorHidePlay(immediately);
		            break;
				
	            default:
		            Debug.LogError("Unknown window animation type: " + _animationType);
		            OnHideComplete();
		            break;
            }
			
            OnHideBegin?.Invoke();
            OnHideBegin?.RemoveAllListeners();
            Debug.LogError(gameObject.name + " hide " + gameObject.activeSelf);
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

		protected virtual void OnTweenAnimationShowPlay(bool isImmediately)
		{
			_windowAnimationTween = DOVirtual.DelayedCall(isImmediately ? 0.0f : 0.5f, OnShowComplete);
		}
		
		protected virtual void OnTweenAnimationHidePlay(bool isImmediately)
		{
			_windowAnimationTween = DOVirtual.DelayedCall(isImmediately ? 0.0f : 0.5f, OnHideComplete);
		}

		protected virtual void OnUnityAnimatorShowPlay(bool isImmediately)
		{
			_windowAnimator.Play(isImmediately ? _idleAnimationName : _showAnimationName);
			if (isImmediately)
				OnShowComplete();
		}

		protected virtual void OnUnityAnimatorHidePlay(bool isImmediately)
		{
			_windowAnimator.Play(isImmediately ? _hiddenAnimationName : _hideAnimationName);
			if (isImmediately)
				OnHideComplete();
		}

		protected virtual void OnShowComplete()
		{
			foreach (CanvasWindowLayer canvasWindowElement in canvasWindowLayers)
				canvasWindowElement.ShowWindowLayerComplete();
			
			OnShowEnd?.Invoke();
			OnShowEnd?.RemoveAllListeners();
		}

		protected virtual void OnHideComplete()
		{
			foreach (CanvasWindowLayer canvasWindowElement in canvasWindowLayers)
				canvasWindowElement.HideWindowLayerComplete();
			
			OnHideEnd?.Invoke();
			OnHideEnd?.RemoveAllListeners();
			
			gameObject.SetActive(false);
			
			Debug.LogError(gameObject.name + " hide complete " + gameObject.activeSelf);
		}
		
		protected virtual void DeInitialize()
		{
			if (_animationType == CanvasWindowsLayerAnimationType.Tween)
				_windowAnimationTween?.Kill(false);
			
			foreach (CanvasWindowLayer canvasWindowLayer in canvasWindowLayers)
			{
				canvasWindowLayer.DeInitialize();
			}
		}

		private void OnDestroy() =>
			DeInitialize();


		public class Factory : PlaceholderFactory<UnityEngine.Object, Transform, CanvasWindow> {}
    }
}
