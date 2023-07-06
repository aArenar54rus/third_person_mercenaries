using UnityEngine;


namespace Arenar.Services.UI
{
	public class AnimatedCanvasWindowLayer : CanvasWindowLayer
	{
		[Header("Animator")]
		[SerializeField] private Animator windowLayerAnimator = default;
		[SerializeField] private string showAnimationName = default;
		[SerializeField] private string idleAnimationName = default;
		[SerializeField] private string hideAnimationName = default;
		[SerializeField] private string hiddenAnimationName = default;


		public override void ShowWindowLayer(bool isImmediately = true)
		{
			if (isImmediately)
			{
				windowLayerAnimator.Play(idleAnimationName);
				base.ShowWindowLayer(isImmediately);
				return;
			}

			if (!Canvas.gameObject.activeSelf)
				Canvas.gameObject.SetActive(true);
			Canvas.enabled = true;
			OnCanvasWindowShowBegin();
			windowLayerAnimator.Play(showAnimationName);
		}

		public override void HideWindowLayer(bool isImmediately = true)
		{
			if (isImmediately)
			{
				windowLayerAnimator.Play(hiddenAnimationName);
				base.HideWindowLayer(isImmediately);
				return;
			}

			OnCanvasWindowHideBegin();
			windowLayerAnimator.Play(hideAnimationName);
		}

		// from animator
		public void OnCanvasWindowShowEndAnimation() =>
			OnCanvasWindowShowEnd();

		// from animator
		public void OnCanvasWindowHideEndAnimation()
		{
			OnCanvasWindowHideEnd();
			Canvas.enabled = false;
		}
	}
}
