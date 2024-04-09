using UnityEngine;


namespace Module.General.Tooltips
{
	public class Example : MonoBehaviour
	{
		[SerializeField] private RectTransform target1;
		[SerializeField] private RectTransform target2;


		void Start()
		{
			Tooltips.ShowTooltip("This is a tooltip 1 R.", target1, Orientation.Right);
			Tooltips.ShowTooltip("This is a tooltip 2 A.", target1, Orientation.Above);
			Tooltips.ShowTooltip("This is a tooltip 3 L.", target1, Orientation.Left);
			Tooltips.ShowTooltip("This is a tooltip 4 B.", target1, Orientation.Below);
			Tooltips.ShowTooltip("This is a tooltip X.", target2, new Vector2(0.85f, 1f));
		}
	}
}
