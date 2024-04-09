using System;
using System.Collections.Generic;
using Module.General.Utils;
using UnityEngine;


namespace Module.General.Tooltips
{
	public class TooltipsSettings : ScriptableSingleton<TooltipsSettings>
	{
		public const string DEFAULT_TEMPLATE_ID = "default";

		#if !UNITY_2020_2_OR_NEWER

		// Hotfix for all old Unity versions
		[Serializable]
		private class PrefabsPair : SerializablePair<string, GameObject>
		{
			public PrefabsPair(string key, GameObject value) : base(key, value) {}
		}

		#endif


		#if UNITY_2020_2_OR_NEWER

		[SerializeField] private List<SerializablePair<string, GameObject>> templates =
			new List<SerializablePair<string, GameObject>>()
				{ new SerializablePair<string, GameObject>(DEFAULT_TEMPLATE_ID, null) };

		#else

		[SerializeField]
		private List<PrefabsPair> templates =
			new List<PrefabsPair>()
				{ new PrefabsPair(DEFAULT_TEMPLATE_ID, null) };

		#endif

		private Dictionary<string, Tooltip> cache = new Dictionary<string, Tooltip>();


		public Tooltip GetTemplate(string id)
		{
			if (!TryGetTemplate(id, out Tooltip prefab))
				TryGetTemplate(DEFAULT_TEMPLATE_ID, out prefab);

			return prefab;
		}


		public bool TryGetTemplate(string id, out Tooltip prefab)
		{
			prefab = null;
			if (!cache.TryGetValue(id, out prefab))
			{
				foreach (var pair in templates)
					if (pair.key == id)
					{
						if (pair.value != null && pair.value.TryGetComponent(out prefab))
						{
							cache.Add(id, prefab);
							break;
						}

						return false;
					}
			}

			return true;
		}
	}
}
