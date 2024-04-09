using UnityEngine;
using UnityEditor;
using System;
using System.Linq;


namespace Module.General
{
	[CustomPropertyDrawer(typeof(InheritorTypeAttribute))]
	public class InheritorTypeAttributePropertyDrawer : PropertyDrawer
	{
		private const string INVALID_TYPE_LABEL = "Attribute invalid for type ";



		protected virtual SerializedProperty GetProperty(SerializedProperty rootProperty) => rootProperty;


		protected virtual Type ObjectType =>
			(attribute as InheritorTypeAttribute)?.type;



		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			property = GetProperty(property);
			if (property.propertyType != SerializedPropertyType.String)
			{
				Rect labelPosition = position;
				labelPosition.width = EditorGUIUtility.labelWidth;
				GUI.Label(labelPosition, label);
				Rect contentPosition = position;
				contentPosition.x += labelPosition.width;
				contentPosition.width -= labelPosition.width;
				EditorGUI.HelpBox(contentPosition, INVALID_TYPE_LABEL + this.fieldInfo.FieldType.Name, MessageType.Error);
			}
			else
			{
				HandleObjectReference(position, property, label);
			}
		}


		private void HandleObjectReference(Rect position, SerializedProperty property, GUIContent label)
		{
			Type[] inheritorTypes = ObjectType.GetImplementations();
			string[] typesStrings = inheritorTypes.Select(x => x.Name).ToArray();

			int selected = Array.IndexOf(typesStrings, property.stringValue);

			selected = selected < 0 ? 0 : selected;

			EditorGUI.BeginChangeCheck();
			{
				selected = EditorGUI.Popup(position, label.text, selected, typesStrings);
				property.stringValue = typesStrings[selected];
			}
			if (EditorGUI.EndChangeCheck())
			{
				property.stringValue = property.stringValue;
			}
		}
	}
}
