using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif


namespace Module.General.AutoReferences
{
	public partial class SceneReferencesContext
	{
		#if UNITY_EDITOR
		private const string SAVE_KEY = "AutoReferencesUpdate";
		private const string DIALOG_TITLE = "AutoReferences System";
		private const string DIALOG_MESSAGE_TEMPLATE = "Auto Update Scene Context {0}";
		private const string DIALOG_MESSAGE_ENABLE= "Enabled";
		private const string DIALOG_MESSAGE_DISABLE = "Disabled";
		private const string DIALOG_BUTTON_OK = "OK";
		private const string FIND_MONO_BEHAVIOURS_REFLECTION_METHOD = nameof(FindMonoBehavioursForAutoReferencesSystem);
		private const bool DEFAULT_SAVE_VALUE = true;
		private const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.NonPublic;

		private static readonly Type ComponentType = typeof(Component);

		[SerializeField] private ReferencesMap map = default;

		private static bool isUpdated = false;

		[MenuItem("Tools/AutoReferences/Toggle Auto Update Scene Context", priority = 0)]
		private static void SwitchAutoUpdateSceneContext()
		{
			if (EditorPrefs.HasKey(SAVE_KEY) == false)
			{
				EditorPrefs.SetBool(SAVE_KEY, DEFAULT_SAVE_VALUE);
			}

			bool isAutoUpdate = EditorPrefs.GetBool(SAVE_KEY);
			isAutoUpdate = !isAutoUpdate;
			EditorPrefs.SetBool(SAVE_KEY, isAutoUpdate);
			string dialogMessage = string.Format(DIALOG_MESSAGE_TEMPLATE, isAutoUpdate ? DIALOG_MESSAGE_ENABLE : DIALOG_MESSAGE_DISABLE);
			EditorUtility.DisplayDialog(DIALOG_TITLE, dialogMessage, DIALOG_BUTTON_OK);
		}

		[DidReloadScripts]
		private static void AutoUpdateSceneContext()
		{
			if (!EditorPrefs.GetBool(SAVE_KEY, DEFAULT_SAVE_VALUE) || isUpdated)
			{
				return;
			}

			isUpdated = true;
			SceneReferencesContext[] sceneContexts = FindObjectsOfType<SceneReferencesContext>();
			foreach (SceneReferencesContext context in sceneContexts)
			{
				MethodInfo methodInfo = typeof(SceneReferencesContext).GetMethod(FIND_MONO_BEHAVIOURS_REFLECTION_METHOD, BINDING_FLAGS);
				methodInfo.Invoke(context, new object[] {});
			}

			isUpdated = false;
		}

		[ContextMenu("Find MonoBehaviours for AutoReferences system")]
		internal void FindMonoBehavioursForAutoReferencesSystem()
		{
			if (map == null)
			{
				throw new ArgumentException("SceneReferencesContext: Map is null");
			}

			map.UpdateReferencesMap();
			AutoReferencesUtility.FindSourcesAndTargets(map, FindObjectsInScene, out sources, out targets);
			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssets();
		}


		private UnityEngine.Object[] FindObjects(Type searchType)
		{
			Type unityType = typeof(UnityEngine.Object);
			Type[] allTypes = Assembly.GetExecutingAssembly().GetTypes();
			var foundTypes = new List<Type>();
			foreach (Type type in allTypes)
			{
				if (unityType.IsAssignableFrom(type) && searchType.IsAssignableFrom(type))
				{
					foundTypes.Add(type);
				}
			}

			var foundObject = new List<UnityEngine.Object>();
			foreach (Type type in foundTypes)
			{
				UnityEngine.Object[] foundArray = FindObjectsInScene(type, map);
				foundObject.AddRange(foundArray);
			}

			return foundObject.ToArray();
		}


		private static UnityEngine.Object[] FindObjectsInScene(Type searchType, ReferencesMap map)
		{
			if (!searchType.IsInterface
				&& !ComponentType.IsAssignableFrom(searchType))
			{
				return Array.Empty<UnityEngine.Object>();
			}

			var foundList = new List<UnityEngine.Object>();
			var rootList = new List<GameObject>();
			Scene scene = SceneManager.GetActiveScene();
			foreach (KeyValuePair<SerializedType, SerializedType> defaultContainer in map.DefaultContainers)
			{
				if (defaultContainer.Key.Type == searchType)
				{
					searchType = defaultContainer.Value.Type;
					break;
				}
			}

			scene.GetRootGameObjects(rootList);
			foreach (GameObject list in rootList)
			{
				foundList.AddRange(list.GetComponentsInChildren(searchType, true));
			}

			return foundList.ToArray();
		}
		#endif
	}
}
