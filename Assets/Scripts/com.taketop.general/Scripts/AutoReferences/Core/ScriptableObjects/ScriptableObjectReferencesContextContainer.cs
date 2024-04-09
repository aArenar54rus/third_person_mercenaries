using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


namespace Module.General.AutoReferences
{
	public class ScriptableObjectReferencesContextContainer : ScriptableSingleton<ScriptableObjectReferencesContextContainer>
	{
		private const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.NonPublic;
		private const string DEFAULT_NAME = "DefaultScriptableObjectContext";
		private const string MAP_FIELD = "map";
		private const string DEFAULT_REFERENCES_MAP_NAME = ReferencesMapContainer.DEFAULT_NAME;

		[SerializeField] private ScriptableList<ScriptableObjectReferencesContext> contexts
			= new ScriptableList<ScriptableObjectReferencesContext>();

		#if UNITY_EDITOR

		public bool TryGetContext(string name, out ScriptableObjectReferencesContext referencesContext)
		{
			if (contexts.Count == 0)
			{
				CreateMap();
			}

			foreach (ScriptableObjectReferencesContext context in contexts)
			{
				referencesContext = context;
				if (referencesContext.name == name)
				{
					return true;
				}
			}

			referencesContext = null;
			return false;
		}


		protected override void OnCreated()
		{
			base.OnCreated();
			CreateMap();
		}


		private void CreateMap()
		{
			var context = CreateInstance<ScriptableObjectReferencesContext>();
			context.name = DEFAULT_NAME;
			if(ReferencesMapContainer.Instance.TryGetReferencesMap(DEFAULT_REFERENCES_MAP_NAME, out ReferencesMap map))
			{
				FieldInfo fieldInfo = typeof(ScriptableObjectReferencesContext).GetField(MAP_FIELD, BINDING_FLAGS);
				fieldInfo.SetValue(context, map);
			}

			AssetDatabase.AddObjectToAsset(context, this);
			contexts.Add(context);
			AssetDatabase.SaveAssets();
		}

		#endif
	}
}
