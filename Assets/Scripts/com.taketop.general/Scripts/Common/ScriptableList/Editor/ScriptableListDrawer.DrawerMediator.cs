using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;


namespace Module.General.Editor
{
	public partial class ScriptableListDrawer
	{
		private abstract class DrawerMediator
		{
			protected DrawerMediator(string label, ReorderableList reorderableList, ScriptableObject scriptableObject)
			{
				Label = label;
				List = reorderableList.list;
				Target = scriptableObject;
			}


			protected string Label { get; }
			protected IList List { get; }
			protected ScriptableObject Target { get; }


			public abstract void DrawCreateField(Rect position);
		}



		private class DrawerMediator<T> : DrawerMediator where T : ScriptableObject
		{
			private const string CREATE_FIELD_NAME = "Create Type";
			private const string ELEMENT_TEMPLATE = "Element {0}";
			private const float OBJECT_WEIGHT = 1f;
			private const float NAME_WEIGHT = 1f;
			private const float SUM_WEIGHT = OBJECT_WEIGHT + NAME_WEIGHT;
			private const float COLUMN_SPACE = 6f;

			private readonly Dictionary<T, UnityEditor.Editor> editors = new Dictionary<T, UnityEditor.Editor>();

			private int lastCreateIndex = -1;


			public DrawerMediator(string label, ReorderableList reorderableList, ScriptableObject scriptableObject) :
				base(label, reorderableList, scriptableObject)
			{
				reorderableList.drawElementCallback += DrawElement;
				reorderableList.drawHeaderCallback += DrawHeader;
				reorderableList.elementHeightCallback += CalculateElementHeight;
				reorderableList.onAddCallback += AddElement;
				reorderableList.onCanAddCallback += CanAdd;
				reorderableList.onRemoveCallback += RemoveElement;
			}


			public override void DrawCreateField(Rect position)
			{
				if (lastCreateIndex > TypeController<T>.Names.Length)
					lastCreateIndex = -1;

				lastCreateIndex =
					EditorGUI.Popup(position, CREATE_FIELD_NAME, lastCreateIndex, TypeController<T>.Names);
			}


			private void AddElement(ReorderableList list)
			{
				string createdTypeName = TypeController<T>.Names[lastCreateIndex];
				lastCreateIndex = -1;
				T elementConfig = TypeController<T>.CreateInstance(createdTypeName);
				elementConfig.name = createdTypeName;
				AssetDatabase.AddObjectToAsset(elementConfig, Target);
				List.Add(elementConfig);
				AssetDatabase.SaveAssets();
			}


			private float CalculateElementHeight(int index)
			{
				float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
				UnityEditor.Editor editor = GetEditor(List[index] as T);
				if (GetOptimizedGUIBlock(editor, false, true, out float editorHeight))
					height += editorHeight;

				return height;
			}


			private bool CanAdd(ReorderableList list) =>
				lastCreateIndex >= 0 && lastCreateIndex < TypeController<T>.Names.Length;


			private void DrawHeader(Rect rect) =>
				EditorGUI.LabelField(rect, Label);


			private void DrawEditor(Rect rect, T value)
			{
				UnityEditor.Editor editor = GetEditor(value);

				if (!GetOptimizedGUIBlock(editor, false, true, out float height))
					return;

				Rect editorRect = rect;
				editorRect.y += EditorGUIUtility.singleLineHeight;
				editorRect.height = height;
				GUI.changed = true;
				OnOptimizedInspectorGUI(editor, editorRect);
			}


			private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
			{
				float weightWidth = (rect.width - EditorGUIUtility.labelWidth - COLUMN_SPACE) / SUM_WEIGHT;
				T value = DrawObjectField(rect, weightWidth, index, out float usedWidth);
				DrawNameField(rect, weightWidth, value, usedWidth);
				DrawEditor(rect, value);
			}


			private T DrawObjectField(Rect rect, float weightWidth, int index, out float usedWidth)
			{
				Rect elementRect = rect;
				elementRect.height = EditorGUIUtility.singleLineHeight;
				elementRect.width = weightWidth * OBJECT_WEIGHT + EditorGUIUtility.labelWidth;
				T value = List[index] as T;
				using (new EditorGUI.DisabledScope(true))
				{
					value = EditorGUI.ObjectField(elementRect, new GUIContent(string.Format(ELEMENT_TEMPLATE, index)),
						value, typeof(T), false) as T;
				}

				usedWidth = elementRect.width;
				return value;
			}


			private static void DrawNameField(Rect rect, float weightWidth, T value, float usedWidth)
			{
				var nameRect = new Rect(
					rect.x + usedWidth + COLUMN_SPACE,
					rect.y,
					weightWidth * NAME_WEIGHT,
					EditorGUIUtility.singleLineHeight
				);

				EditorGUI.BeginChangeCheck();
				string valueName = EditorGUI.TextField(nameRect, GUIContent.none, value.name);

				if (!EditorGUI.EndChangeCheck() || value.name == valueName)
					return;

				value.name = valueName;
				AssetDatabase.SaveAssets();
				EditorGUIUtility.PingObject(value);
			}


			private UnityEditor.Editor GetEditor(T value)
			{
				if (editors.TryGetValue(value, out UnityEditor.Editor editor))
					return editor;

				editor = UnityEditor.Editor.CreateEditor(value as T);
				editors.Add(value, editor);

				return editor;
			}


			private void RemoveElement(ReorderableList list)
			{
				int removedIndex = list.index;
				T removedObject = List[removedIndex] as T;
				List.RemoveAt(removedIndex);

				if (removedObject == null)
					return;

				if (editors.TryGetValue(removedObject, out UnityEditor.Editor editor))
				{
					editors.Remove(removedObject);
					Object.DestroyImmediate(editor);
				}

				Object.DestroyImmediate(removedObject, true);
				AssetDatabase.SaveAssets();
			}
		}
	}
}
