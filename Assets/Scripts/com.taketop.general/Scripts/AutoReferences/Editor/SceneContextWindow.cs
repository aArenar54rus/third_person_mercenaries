#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace Module.General.AutoReferences
{
	public class SceneContextWindow : EditorWindow
	{
		private const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.NonPublic;
		private const string MAP_FIELD = "map";
		private const string SOURCE_OBJECTS_FIELD = nameof(SceneReferencesContext.sources);
		private const string TARGET_OBJECTS_FIELD = nameof(SceneReferencesContext.targets);
		private const string WINDOW_NAME = "Scene Context Edit";

		[SerializeField] private ReferencesMap referencesMap = default;
		[SerializeField] private List<SerializedSource> sources = default;
		[SerializeField] private List<SerializedTarget> targets = default;

		private static SceneReferencesContext currentReferencesContext;


		[MenuItem("Tools/AutoReferences/Edit Scene Context", priority = 2)]
		private static void ShowWindow()
		{
			currentReferencesContext = FindObjectOfType<SceneReferencesContext>();

			var window = GetWindow<SceneContextWindow>(WINDOW_NAME);
			window.Show();
		}


		private void OnEnable()
		{
			currentReferencesContext = FindObjectOfType<SceneReferencesContext>();
		}


		private void OnGUI()
		{
			if (currentReferencesContext == null)
			{
				return;
			}

			FieldInfo mapFieldInfo = GetFieldReflection(MAP_FIELD);
			referencesMap = (ReferencesMap)mapFieldInfo.GetValue(currentReferencesContext);
			GetFields();
			var serializedObject = new SerializedObject(this);
			SerializedProperty serializedPropertySources = serializedObject.FindProperty(SOURCE_OBJECTS_FIELD);
			SerializedProperty serializedPropertyTargets = serializedObject.FindProperty(TARGET_OBJECTS_FIELD);

			using (new EditorGUI.DisabledScope(false))
			{
				referencesMap = EditorGUILayout.ObjectField("References Map:", referencesMap, typeof(ReferencesMap)) as ReferencesMap;
				EditorGUILayout.PropertyField(serializedPropertySources);
				EditorGUILayout.PropertyField(serializedPropertyTargets);
			}

			if (GUILayout.Button("Save"))
			{
				mapFieldInfo.SetValue(currentReferencesContext, referencesMap);
				EditorUtility.SetDirty(currentReferencesContext);
			}
		}


		private void GetFields()
		{
			sources = new List<SerializedSource>();
			targets = new List<SerializedTarget>();
			FieldInfo fieldInfo = GetFieldReflection(SOURCE_OBJECTS_FIELD);
			sources.AddRange((SerializedSource[])fieldInfo.GetValue(currentReferencesContext));
			fieldInfo = GetFieldReflection(TARGET_OBJECTS_FIELD);
			targets.AddRange((SerializedTarget[])fieldInfo.GetValue(currentReferencesContext));
		}


		private static FieldInfo GetFieldReflection(string fieldName)
			=> typeof(SceneReferencesContext).GetField(fieldName, BINDING_FLAGS);
	}
}
#endif
