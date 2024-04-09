using System;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Module.General.Editor
{
	[Obsolete]
	[CustomPropertyDrawer(typeof(ScriptableList<>))]
	public partial class ScriptableListDrawer : PropertyDrawer
	{
		private const float CREATE_FIELD_RIGHT_OFFSET = 100f;

		private const string ERROR_MESSAGE =
			"ScriptableList can only be used within the inheritors of ScriptableObject";

		private const float ERROR_MESSAGE_HEIGHT = 25F;

		private static readonly Type ScriptableListTypeDefinition = typeof(ScriptableList<>);

		private ReorderableList reorderable;
		private DrawerMediator drawerMediator;


		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Init(property);
			if (reorderable == null)
			{
				EditorGUI.HelpBox(position, ERROR_MESSAGE, MessageType.Error);
				return;
			}

			reorderable.DoList(position);
			position.y += reorderable.GetHeight() - EditorGUIUtility.singleLineHeight;
			position.height = EditorGUIUtility.singleLineHeight;
			position.width -= CREATE_FIELD_RIGHT_OFFSET;
			drawerMediator.DrawCreateField(position);
		}


		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			Init(property);

			if (reorderable != null)
				return reorderable.GetHeight() + EditorGUIUtility.standardVerticalSpacing;

			return ERROR_MESSAGE_HEIGHT;
		}


		private void Init(SerializedProperty property)
		{
			if (reorderable != null ||
				!(property.serializedObject.targetObject is ScriptableObject targetObject))
				return;

			ValidateGenericType(fieldInfo.FieldType, out Type listArgumentType);
			IList list = fieldInfo.GetValue(property.serializedObject.targetObject) as IList;
			reorderable = new ReorderableList(list, listArgumentType, true, true, true, true);
			drawerMediator = CreateMediator(property.displayName, listArgumentType, reorderable, targetObject);
		}
	}
}
