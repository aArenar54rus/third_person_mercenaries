#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using Object = UnityEngine.Object;


namespace Module.General
{
	public abstract partial class SelfInstancingMono : IPreprocessBuildWithReport
	{
		#region IPreprocessBuildWithReport

		int IOrderedCallback.callbackOrder => 0;

		void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
		{
			if (ShouldBeCreatedFromPrefab && Prefab == null)
			{
				TryFindAndAttachPrefab(GetType());
			}

			OnPrebuildProcess();
		}

		#endregion



		#region Static Methods

		private static void TryFindAndAttachPrefab(Type type)
		{
			List<GameObject> prefabs = FindAssets(type);
			GameObject prefab = prefabs.Count > 0 ? prefabs[0] : null;

			SetPrefab(type, prefab);
		}


		private static List<GameObject> FindAssets(Type assetType)
		{
			string[] guids = AssetDatabase.FindAssets("t:GameObject");
			string searchableType = assetType.Name;
			var res = new List<GameObject>();

			foreach (string guid in guids)
			{
				string myObjectPath = AssetDatabase.GUIDToAssetPath(guid);
				Object[] myObjs = AssetDatabase.LoadAllAssetsAtPath(myObjectPath);

				foreach (Object item in myObjs)
				{
					if (item is null)
					{
						Debug.LogWarning($"Empty component inside prefab {myObjectPath}", AssetDatabase.LoadAssetAtPath<GameObject>(myObjectPath));
						continue;
					}

					string itemType = item.GetType().Name;

					if (itemType == searchableType)
					{
						var go = AssetDatabase.LoadAssetAtPath<GameObject>(myObjectPath);
						res.Add(go);
						continue;
					}
				}
			}

			return res;
		}


		private static void SetPrefab(Type type, GameObject prefab)
		{
			var typeName = type.ToString();

			if (!SelfInstancingPrefabs.Instance.ContainsKey(typeName))
			{
				SelfInstancingPrefabs.Instance.Add(typeName, null);
			}

			SelfInstancingPrefabs.Instance.Set(typeName, prefab);

			AssetDatabase.SaveAssets();
		}

		#endregion



		#region Methods

		protected virtual void OnPrebuildProcess() {}


		protected void RemovePrefab()
		{
			var typeName = GetType().ToString();

			if (!SelfInstancingPrefabs.Instance.ContainsKey(typeName))
				return;

			SelfInstancingPrefabs.Instance.Remove(typeName);

			AssetDatabase.SaveAssets();
		}

		#endregion
	}
}
#endif
