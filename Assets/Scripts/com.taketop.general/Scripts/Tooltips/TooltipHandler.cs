using UnityEngine;


namespace Module.General.Tooltips
{
	public class TooltipHandler : MonoBehaviour
	{
		[SerializeField] private RectTransform tooltipParentContainer;
		[SerializeField] private Camera customUICamera;


		private void Awake()
		{
			Tooltips.SetParent(tooltipParentContainer);
			Tooltips.SetUICamera(customUICamera);
		}
	}
}
