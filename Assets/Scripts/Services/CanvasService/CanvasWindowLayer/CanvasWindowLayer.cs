using System;
using DG.Tweening;
using UnityEngine;


namespace Arenar.Services.UI
{
    public abstract class CanvasWindowLayer : MonoBehaviour
    {
	    [SerializeField] private CanvasWindowsLayerAnimationType _animationType;
	    [SerializeField] private Animator _windowAnimator;
	    [SerializeField] private string _showAnimationName = default;
	    [SerializeField] private string _idleAnimationName = default;
	    [SerializeField] private string _hideAnimationName = default;
	    [SerializeField] private string _hiddenAnimationName = default;

	    protected Tween _windowAnimationTween;
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

		public void ShowWindowLayer(bool isImmediately = true)
		{
			OnCanvasWindowShowBegin();

			switch (_animationType)
			{
				case CanvasWindowsLayerAnimationType.None:
					OnCanvasWindowShowEnd();
					break;
				
				case CanvasWindowsLayerAnimationType.Tween:
					OnTweenAnimationShowPlay(isImmediately);
					break;
				
				case CanvasWindowsLayerAnimationType.ClassicUnity:
					OnUnityAnimatorShowPlay(isImmediately);
					break;
				
				default:
					Debug.LogError("Unknown window animation type: " + _animationType);
					OnCanvasWindowShowEnd();
					break;
			}
		}

		public void HideWindowLayer(bool isImmediately = true)
		{
			OnCanvasWindowHideBegin();

			switch (_animationType)
			{
				case CanvasWindowsLayerAnimationType.None:
					OnCanvasWindowHideEnd();
					break;
				
				case CanvasWindowsLayerAnimationType.Tween:
					OnTweenAnimationHidePlay(isImmediately);
					break;
				
				case CanvasWindowsLayerAnimationType.ClassicUnity:
					OnUnityAnimatorHidePlay(isImmediately);
					
					break;
				
				default:
					Debug.LogError("Unknown window animation type: " + _animationType);
					OnCanvasWindowShowEnd();
					break;
			}
			
			OnCanvasWindowHideEnd();
		}
		
		public virtual void OnCanvasWindowShowEnd()
		{
			onCanvasLayerShowEnd?.Invoke(this);
		}

		public virtual void OnCanvasWindowHideBegin()
		{
			onCanvasLayerHideBegin?.Invoke(this);
		}

		protected virtual void OnTweenAnimationShowPlay(bool isImmediately)
		{
			_windowAnimationTween = DOVirtual.DelayedCall(isImmediately ? 0.0f : 0.5f, OnCanvasWindowShowEnd);
		}
		
		protected virtual void OnTweenAnimationHidePlay(bool isImmediately)
		{
			_windowAnimationTween = DOVirtual.DelayedCall(isImmediately ? 0.0f : 0.5f, OnCanvasWindowHideEnd);
		}

		protected virtual void OnUnityAnimatorShowPlay(bool isImmediately)
		{
			_windowAnimator.Play(isImmediately ? _idleAnimationName : _showAnimationName);
			if (isImmediately)
				OnCanvasWindowShowEnd();
		}

		protected virtual void OnUnityAnimatorHidePlay(bool isImmediately)
		{
			_windowAnimator.Play(isImmediately ? _hiddenAnimationName : _hideAnimationName);
			if (isImmediately)
				OnCanvasWindowShowEnd();
		}

		protected virtual void OnCanvasWindowShowBegin()
		{
			Canvas.enabled = true;
			onCanvasLayerShowBegin?.Invoke(this);
		}
		
		protected virtual void OnCanvasWindowHideEnd()
		{
			onCanvasLayerHideEnd?.Invoke(this);
			Canvas.enabled = false;
		}

		protected void OnDestroy()
		{
			if (_animationType != CanvasWindowsLayerAnimationType.Tween)
				return;
			
			_windowAnimationTween?.Kill(false);
		}
    }
}
