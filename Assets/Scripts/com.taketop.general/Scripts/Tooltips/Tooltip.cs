using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Module.General.Tooltips
{
	public class Tooltip : MonoBehaviour
	{
		#region Fields

		[SerializeField] protected Text textObject;
		[SerializeField] protected TMP_Text tmpObject;
		[SerializeField] protected RectTransform tip;
		[SerializeField] protected bool useTip = true;
		[SerializeField] protected Settings defaultSettings = new Settings();


		protected float tipLength;
		protected Settings settings;

		protected RectTransform rectTransform;
		protected ContentSizeFitter sizeFitter;
		protected ContentSizeFitter textSizeFitter;
		protected HorizontalOrVerticalLayoutGroup layoutGroup;

		protected RectTransform target;
		protected RectTransform parent;
		protected Vector2 pivot;
		protected Orientation orientation;

		protected readonly Dictionary<Orientation, float> tipRotationAngle = new Dictionary<Orientation, float>()
		{
			[Orientation.Above] = 0f,
			[Orientation.Right] = -90f,
			[Orientation.Below] = -180f,
			[Orientation.Left] = -270f,
		};

		#endregion



		#region Properties

		public AutoSizeSettings AutoSize
		{
			get => settings.autosize;
			set
			{
				settings.autosize = value;
				ReCalculateSize();
			}
		}


		public bool UseTip
		{
			get => useTip;
			set
			{
				if (tip == null)
				{
					Debug.LogWarning("Tooltip doesn't have tip. \"UseTip\" set to false.", this);
					useTip = false;
					tipLength = 0f;
					return;
				}

				useTip = value;
				tipLength = tip.sizeDelta.y * tip.pivot.y;
			}
		}

		public float TextSize
		{
			get => settings.textSize;
			set
			{
				settings.textSize = value;
				ReCalculateSize();
			}
		}

		public Vector2 Size
		{
			get => settings.size;
			set
			{
				settings.size = value;
				ReCalculateSize();
			}
		}

		public float OffsetTarget
		{
			get => settings.targetOffset;
			set
			{
				settings.targetOffset = value;
				RecalculatePosition();
			}
		}

		public float OffsetScreen
		{
			get => settings.screenOffset;
			set
			{
				settings.screenOffset = value;
				RecalculatePosition();
			}
		}

		#endregion



		#region Methods

		protected virtual void Awake()
		{
			rectTransform = GetComponent<RectTransform>();
			sizeFitter = GetComponent<ContentSizeFitter>();
			layoutGroup = GetComponent<HorizontalOrVerticalLayoutGroup>();
			UseTip = useTip;
			if (textObject)
				textSizeFitter = textObject.GetComponent<ContentSizeFitter>();

			Initialize();
		}


		protected internal virtual void Initialize()
		{
			settings = defaultSettings;
			parent = rectTransform.parent.GetComponent<RectTransform>();
			ReCalculateSize();
		}


		public virtual void SetText(string text)
		{
			textObject.text = text;
			RecalculatePosition();
		}


		public void SetPivot(Vector2 pivot)
		{
			if (!Tooltips.IsPivotValid(pivot))
			{
				Debug.LogError($"Wrong pivot ${pivot}!", this);
				return;
			}

			this.pivot = pivot;
			rectTransform.pivot = pivot;
			RecalculatePosition();
		}


		public virtual void SetTarget(RectTransform target)
		{
			this.target = target;
			RecalculatePosition();
		}


		protected virtual void ReCalculateSize()
		{
			// textObject.resizeTextForBestFit = false;
			// textObject.fontSize = (int)settings.textSize;
			//
			// layoutGroup.childControlWidth = true;
			// layoutGroup.childControlHeight = true;
			//
			//
			// rectTransform.sizeDelta = settings.size;
			//
			// textSizeFitter.enabled = true;
			// sizeFitter.enabled = false;

			switch (settings.autosize)
			{
				case AutoSizeSettings.TextSize:
					layoutGroup.childControlWidth = true;
					layoutGroup.childControlHeight = true;

					rectTransform.sizeDelta = settings.size;

					sizeFitter.enabled = false;
					textSizeFitter.enabled = false;

					textObject.resizeTextForBestFit = true;
					break;

				case AutoSizeSettings.RectSize:
					layoutGroup.childControlWidth = false;
					layoutGroup.childControlHeight = false;

					textSizeFitter.enabled = true;

					textObject.resizeTextForBestFit = false;
					textObject.fontSize = (int)settings.textSize;

					sizeFitter.enabled = true;
					break;

				case AutoSizeSettings.None:
					layoutGroup.childControlWidth = true;
					layoutGroup.childControlHeight = true;

					textObject.resizeTextForBestFit = false;
					rectTransform.sizeDelta = settings.size;

					textObject.fontSize = (int)settings.textSize;

					sizeFitter.enabled = false;
					textSizeFitter.enabled = true;
					break;

				default:
					break;
			}
		}


		protected virtual void RecalculatePosition()
		{
			if (Tooltips.IsPivotValid(pivot) == false || target == null)
				return;

			// 0. Offset
			var offset = pivot;
			for (int i = 0; i < 2; i++)
				offset[i] = offset[i] == 0f || offset[i] == 1f ? 1f : 0f;

			offset = new Vector2(
				offset.x * (OffsetTarget + tipLength) * (pivot.x == 1f ? -1f : 1f),
				offset.y * (OffsetTarget + tipLength) * (pivot.y == 1f ? -1f : 1f)
			);

			// 1. Tip
			if (UseTip)
			{
				Tooltips.TryPivotToOrientation(pivot, out Orientation orientation);
				var angle = tipRotationAngle[orientation];
				tip.localEulerAngles = Vector3.forward * angle;
				tip.anchorMin = pivot;
				tip.anchorMax = pivot;
				tip.anchoredPosition = Vector2.zero;
			}

			// 2. Set over target
			var bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(parent, target);
			rectTransform.anchoredPosition = offset + (Vector2)bounds.center;

			// 3. Screen borders cutting


		}

		#endregion
	}
}
