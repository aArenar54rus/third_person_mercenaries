using System;
using System.Collections.Generic;
using Module.General.Utils;
using UnityEngine;


namespace Module.General
{
	public class SelfInstancingPrefabs : ScriptableSingleton<SelfInstancingPrefabs>
	{
		#if !UNITY_2020_2_OR_NEWER

		// Hotfix for all old Unity versions
		[Serializable]
		private class PrefabsPair : SerializablePair<string, GameObject>
		{
			public PrefabsPair(string key, GameObject value) : base(key, value) {}
		}

		#endif


		#if UNITY_2020_2_OR_NEWER

		[SerializeField] SerializableDictionary<string, GameObject> prefabsSet = new SerializableDictionary<string, GameObject>();

		#else

		[SerializeField] List<PrefabsPair> prefabsSet = new List<PrefabsPair>();

		#endif


		public bool ContainsKey(string key)
		{
			#if UNITY_2020_2_OR_NEWER
			return prefabsSet.ContainsKey(key);
			#else
			return GetIndex(key) > -1;
			#endif
		}


		public void Add(string typeName, GameObject prefab)
		{
			#if UNITY_2020_2_OR_NEWER
			prefabsSet.Add(typeName, prefab);
			#else
			prefabsSet.Add(new PrefabsPair(typeName, prefab));
			#endif
		}


		public void Set(string typeName, GameObject prefab)
		{
			#if UNITY_2020_2_OR_NEWER
			prefabsSet[typeName] = prefab;
			#else
			if (ContainsKey(typeName))
				Remove(typeName);

			prefabsSet.Add(new PrefabsPair(typeName, prefab));
			#endif
		}


		public void Remove(string typeName)
		{
			#if UNITY_2020_2_OR_NEWER
			prefabsSet.Remove(typeName);
			#else
			var index = GetIndex(typeName);
			if (index > -1)
				prefabsSet.RemoveAt(index);
			#endif
		}


		public bool TryGetValue(string typeName, out GameObject prefab)
		{
			#if UNITY_2020_2_OR_NEWER
			return prefabsSet.TryGetValue(typeName, out prefab);
			#else
			var index = GetIndex(typeName);
			if (index > -1)
			{
				prefab = prefabsSet[index].value;
				return true;
			}

			prefab = null;
			return false;
			#endif
		}


		#if !UNITY_2020_2_OR_NEWER
		private int GetIndex(string key)
		{
			for (int i = 0, count = prefabsSet.Count; i < count; i++)
				if (prefabsSet[i].key == key)
					return i;

			return -1;
		}
		#endif
	}
}
