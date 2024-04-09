using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Module.General
{
	public abstract partial class SelfInstancingMono : MonoBehaviour
	{
		#region Properties

		protected bool IsEmptyObject => transform.childCount == 0;

		protected virtual bool ShouldCreateSelfInstanceAutomatically => true;

		protected virtual bool ShouldBeCreatedFromPrefab => false;

		protected GameObject Prefab =>
			GetPrefab(GetType());

		#endregion



		#region Static Methods

		private static GameObject GetPrefab(Type type)
		{
			SelfInstancingPrefabs.Instance.TryGetValue(type.ToString(), out GameObject prefab);
			return prefab;
		}


		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void CreateInstances()
		{
			foreach (Type type in typeof(SelfInstancingMono).GetImplementations())
			{
				try
				{
					SelfInstancingMono instance = CreateSelfInstance(type);
					if (instance != null)
					{
						instance.Initialize();
						instance.Launch();
					}
				}
				catch (Exception e)
				{
					preloadLogs.Add(e.ToString());
				}
			}

			if (preloadLogs.Count > 0)
			{
				CreateLogsOutput();
			}
		}

		private void Launch()
		{
			StartCoroutine(DeferredLaunch());
		}


		private IEnumerator DeferredLaunch()
		{
			yield return 0f;
			OnLaunch();
		}


		protected static T CreateSelfInstance<T>() where T : SelfInstancingMono
		{
			Type selfType = typeof(T);

			Object obj = FindObjectOfType(selfType);

			if (obj != null)
			{
				return (T)obj;
			}

			GameObject go = CreateEmpty(selfType);

			var component = (T)go.GetComponent<SelfInstancingMono>();

			if (!component.ShouldCreateSelfInstanceAutomatically)
			{
				Destroy(go);
				return null;
			}

			if (!component.ShouldBeCreatedFromPrefab)
			{
				return component;
			}

			#if UNITY_EDITOR
			if (component.Prefab == null)
			{
				TryFindAndAttachPrefab(selfType);
			}
			#endif

			GameObject prefab = component.TryInstantiatePrefab();

			if (prefab == null)
			{
				return component;
			}

			Destroy(go);

			return (T)prefab.GetComponent(selfType);
		}


		private static SelfInstancingMono CreateSelfInstance(Type type)
		{
			Object obj = FindObjectOfType(type);

			if (obj != null)
			{
				return obj as SelfInstancingMono;
			}

			GameObject go = CreateEmpty(type);

			var component = go.GetComponent<SelfInstancingMono>();

			if (!component.ShouldCreateSelfInstanceAutomatically)
			{
				Destroy(go);
				return null;
			}

			if (!component.ShouldBeCreatedFromPrefab)
			{
				return component;
			}

			#if UNITY_EDITOR
			if (component.Prefab == null)
			{
				TryFindAndAttachPrefab(type);
			}
			#endif

			GameObject prefab = component.TryInstantiatePrefab();

			if (prefab == null)
			{
				return component;
			}

			Destroy(go);

			return prefab.GetComponent<SelfInstancingMono>();
		}


		private static GameObject CreateEmpty(Type type) =>
			new GameObject(type.Name, type);

		#endregion



		#region Methods

		private GameObject TryInstantiatePrefab() =>
			Prefab == null ? null : Instantiate(Prefab);


		protected virtual void Initialize() {}


		protected virtual void OnLaunch() {}

		#endregion
	}
}
