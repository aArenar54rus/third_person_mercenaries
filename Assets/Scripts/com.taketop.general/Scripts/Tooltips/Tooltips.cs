using UnityEngine;


namespace Module.General.Tooltips
{
	public class Tooltips : MonoSingleton<Tooltips>
	{
		private RectTransform tooltipParentContainer;


		public static void SetParent(RectTransform parent)
		{
			Instance.tooltipParentContainer = parent;
		}


		public static void SetUICamera(Camera camera)
		{
			if (camera == null)
				_ = Camera.main;
		}


		public static Tooltip ShowTooltip(string text, RectTransform target, Orientation orientation = Orientation.Below, string template = TooltipsSettings.DEFAULT_TEMPLATE_ID)
		{
			Vector2 pivot = OrientationToPivot(orientation);

			return ShowTooltip(text, target, pivot, template);
		}


		public static Tooltip ShowTooltip(string text, RectTransform target, Vector2 pivot, string template = TooltipsSettings.DEFAULT_TEMPLATE_ID)
		{
			if (target == null)
			{
				Debug.LogError($"You didn't set target for tooltip!");
				return null;
			}

			Tooltip prefab = TooltipsSettings.Instance.GetTemplate(template);
			if (prefab == null)
			{
				Debug.LogError($"You don't have neither templates \"{template}\" or default. Do it in <b>TooltipsSettings</b> at <i>Resources/TooltipsSettings</i>");
				return null;
			}

			Tooltip tooltip = Instantiate(prefab, Instance.tooltipParentContainer);
			tooltip.SetText(text);
			tooltip.SetPivot(pivot);
			tooltip.SetTarget(target);
			tooltip.Initialize();

			return tooltip;
		}


		public static Vector2 OrientationToPivot(Orientation orientation)
		{
			switch (orientation)
			{
				case Orientation.Left:
					return new Vector2(1f, 0.5f);

				case Orientation.Right:
					return new Vector2(0f, 0.5f);

				case Orientation.Below:
					return new Vector2(0.5f, 1f);

				case Orientation.Above:
				default:
					return new Vector2(0.5f, 0f);
			}
		}


		public static bool TryPivotToOrientation(Vector2 pivot, out Orientation orientation)
		{
			orientation = default;

			if (!IsPivotValid(pivot))
				return false;

			if (pivot.x == 0f) orientation = Orientation.Right;
			else if (pivot.x == 1f) orientation = Orientation.Left;
			else if (pivot.y == 0f) orientation = Orientation.Above;
			else if (pivot.y == 1f) orientation = Orientation.Below;

			return true;
		}


		public static bool IsPivotValid(Vector2 p)
		{
			return (p.x == 0f || p.x == 1f) && p.y > 0f && p.y < 1f
				   || (p.y == 0f || p.y == 1f) && p.x > 0f && p.x < 1f;
				   // || p.x > 0f && p.x < 1f && p.y > 0f && p.y < 1f;
		}
	}
}
