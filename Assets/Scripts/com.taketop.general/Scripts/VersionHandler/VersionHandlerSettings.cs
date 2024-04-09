using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Module.General.VersionHandler
{
	public class VersionHandlerSettings : ScriptableSingleton<VersionHandlerSettings>
	{
		[SerializeField] private List<ScriptableObject> handlers = new List<ScriptableObject>();

		public IEnumerable<IVersionChangeHandler> Handlers =>
			handlers.ConvertAll(input => (IVersionChangeHandler)input);



		#region Editor Methods

		#if UNITY_EDITOR

		protected override void OnCreated()
		{
			base.OnCreated();
			FindWidgetsCollections();
		}


		[ContextMenu("Find All Processors")]
		private void FindWidgetsCollections()
		{
			CheckAssetsDatabase();

			EditorUtility.SetDirty(this);

			AssetDatabase.SaveAssets();


			void CheckAssetsDatabase()
			{
				string[] guids = AssetDatabase.FindAssets("t:ScriptableObject");

				foreach (var guid in guids)
				{
					string path = AssetDatabase.GUIDToAssetPath(guid);
					var asset = (ScriptableObject)AssetDatabase.LoadAssetAtPath(path, typeof(IVersionChangeHandler));

					if (asset != null && !handlers.Contains(asset))
						handlers.Add(asset);
				}
			}
		}


		private void OnValidate()
		{
			foreach (var key in handlers.ToArray())
			{
				if (key == null || key is IVersionChangeHandler)
					continue;

				handlers.Remove(key);
			}
		}
		#endif

		#endregion
	}
}
