using UnityEngine;
using UnityEditor;


namespace Module.General
{
	[CustomPropertyDrawer(typeof(ColorHtmlPropertyAttribute))]
	public class ColorAttributePropertyDrawer : PropertyDrawer
	{
		protected virtual SerializedProperty GetProperty(SerializedProperty rootProperty) => rootProperty;


		protected virtual Color RequiredColor =>
			((ColorHtmlPropertyAttribute)attribute).Color;
		protected virtual TypeHighlighting TypeHighlighting =>
			((ColorHtmlPropertyAttribute)attribute).TypeHighlighting;


		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			property = GetProperty(property);

			HandleObjectReference(position, property, label);
		}


		private void HandleObjectReference(Rect position, SerializedProperty property, GUIContent label)
		{
			Color oldColor = GUI.color;

			switch (TypeHighlighting)
			{
				case TypeHighlighting.All:
					GUI.color = RequiredColor;
					EditorGUI.PropertyField(position, property, label);
					GUI.color = oldColor;
					break;

				case TypeHighlighting.OnlyField:
					GUI.backgroundColor = RequiredColor;
					EditorGUI.PropertyField(position, property, label);
					GUI.backgroundColor = oldColor;
					break;
			}
		}
	}
}
