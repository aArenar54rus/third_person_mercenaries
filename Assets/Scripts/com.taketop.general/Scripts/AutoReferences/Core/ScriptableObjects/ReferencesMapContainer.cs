#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


namespace Module.General.AutoReferences
{
	public class ReferencesMapContainer : ScriptableSingleton<ReferencesMapContainer>
	{
		public const string DEFAULT_NAME = "DefaultReferencesMap";

		[SerializeField] private ScriptableList<ReferencesMap> maps = new ScriptableList<ReferencesMap>();

		#if UNITY_EDITOR

		public bool TryGetReferencesMap(string name, out ReferencesMap map)
		{
			if (maps.Count == 0)
			{
				CreateMap();
			}

			foreach (ReferencesMap m in maps)
			{
				map = m;
				if (map.name == name)
				{
					return true;
				}
			}

			map = null;
			return false;
		}


		protected override void OnCreated()
		{
			base.OnCreated();
			CreateMap();
		}


		private void CreateMap()
		{
			var config = CreateInstance(typeof(ReferencesMap)) as ReferencesMap;
			config.name = DEFAULT_NAME;
			AssetDatabase.AddObjectToAsset(config, this);
			maps.Add(config);
			AssetDatabase.SaveAssets();
		}

		#endif
	}
}
