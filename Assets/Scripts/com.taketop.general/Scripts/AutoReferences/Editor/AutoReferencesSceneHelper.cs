#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace Module.General.AutoReferences
{
	public class AutoReferencesSceneHelper : UnityEditor.Editor
	{
		private const BindingFlags BINDING_FLAGS = BindingFlags.NonPublic | BindingFlags.Instance;
		private const string SCENE_CONTEXT_NAME = "References Scene Context";
		private const string REFERENCES_MAP = "map";
		private const string FIND_MONO_BEHAVIOURS_REFLECTION_METHOD = nameof(SceneReferencesContext.FindMonoBehavioursForAutoReferencesSystem);
		private const string DEFAULT_REFERENCES_MAP_NAME = ReferencesMapContainer.DEFAULT_NAME;

		private static FieldInfo ReflectionGetField<TObject>(string fieldName)
		{
			return typeof(TObject).GetField(fieldName, BINDING_FLAGS);
		}


		private static void ReflectionSetField<TObject>(string fieldName, object instance, object value)
		{
			FieldInfo fieldInfo = ReflectionGetField<TObject>(fieldName);
			fieldInfo.SetValue(instance, value);
		}


		private static TObject CreateObject<TObject>(string name) where TObject : MonoBehaviour
		{
			return new GameObject(name).AddComponent<TObject>();
		}


		[MenuItem("Tools/AutoReferences/Bake", priority = 1)]
		private static void BakeScene()
		{
			var sceneContext = FindObjectOfType<SceneReferencesContext>();
			if (sceneContext == null)
			{
				CustomDebug.LogError("There is no Scene Context in current scene!");
				return;
			}

			MethodInfo methodInfo = typeof(SceneReferencesContext).GetMethod(FIND_MONO_BEHAVIOURS_REFLECTION_METHOD, BINDING_FLAGS);
			methodInfo.Invoke(sceneContext, null);
		}


		[MenuItem("Tools/AutoReferences/Include Scene", priority = 3)]
		private static void AddSceneContext()
		{
			RemoveSceneContext();
			var sceneContext = CreateObject<SceneReferencesContext>(SCENE_CONTEXT_NAME);
			sceneContext.transform.SetAsFirstSibling();
			if (ReferencesMapContainer.Instance.TryGetReferencesMap(DEFAULT_REFERENCES_MAP_NAME, out ReferencesMap map))
			{
				ReflectionSetField<SceneReferencesContext>(REFERENCES_MAP, sceneContext, map);
			}
			sceneContext.FindMonoBehavioursForAutoReferencesSystem();

			EditorUtility.SetDirty(sceneContext);
		}


		[MenuItem("Tools/AutoReferences/Exclude Scene", priority = 4)]
		private static void RemoveSceneContext()
		{
			SceneReferencesContext[] sceneContexts = FindObjectsOfType<SceneReferencesContext>();

			foreach (SceneReferencesContext context in sceneContexts)
			{
				DestroyImmediate(context);
			}
		}
	}
}
#endif
