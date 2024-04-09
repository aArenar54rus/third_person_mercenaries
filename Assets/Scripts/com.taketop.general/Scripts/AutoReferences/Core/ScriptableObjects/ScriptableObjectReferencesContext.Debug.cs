using Object = UnityEngine.Object;
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#endif


namespace Module.General.AutoReferences
{
	public partial class ScriptableObjectReferencesContext
	{
		#if UNITY_EDITOR
		private const string AssetsFilter = "t:ScriptableObject";

		[SerializeField] private ReferencesMap map;
		[SerializeField] private DefaultAsset[] foldersForSearch;


		[ContextMenu("Find ScriptableObjects")]
		internal void FindScriptableObjects()
		{
			List<ScriptableObject> foundScriptableObjects = GetScriptableObjectsInFolders();
			AutoReferencesUtility.FindSourcesAndTargets(map, FindObjectsFunction, out sources, out targets);

			Object[] FindObjectsFunction(Type type, ReferencesMap map)
			{
				var foundObjects = new List<UnityEngine.Object>();
				foreach (ScriptableObject obj in foundScriptableObjects)
				{
					if (type.IsInstanceOfType(obj))
					{
						foundObjects.Add(obj);
					}
				}

				return foundObjects.ToArray();
			}
		}


		private List<ScriptableObject> GetScriptableObjectsInFolders()
		{
			var foundObjects = new List<ScriptableObject>();
			var foundPaths = new List<string>();

			var folders = new DefaultAsset[foldersForSearch.Length];
			foldersForSearch.CopyTo(folders, 0);

			if (folders == null || folders.Length == 0)
			{
				folders = new[] { AssetDatabase.LoadAssetAtPath<DefaultAsset>("Assets") };
			}

			foreach (DefaultAsset folderForSearch in folders)
			{
				if (folderForSearch == null)
				{
					continue;
				}

				string searchPath = AssetDatabase.GetAssetPath(folderForSearch);

				if (!string.IsNullOrEmpty(searchPath) && !foundPaths.Contains(searchPath))
				{
					foundPaths.Add(searchPath);
				}
			}

			string[] guids = AssetDatabase.FindAssets(AssetsFilter, foundPaths.ToArray());
			foreach (string guid in guids)
			{
				string path = AssetDatabase.GUIDToAssetPath(guid);
				UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);

				foreach (Object asset in assets)
				{
					if (asset is ScriptableObject scriptableObject && !foundObjects.Contains(scriptableObject))
					{
						foundObjects.Add(scriptableObject);
					}
				}
			}

			return foundObjects;
		}
		#endif
	}
}
