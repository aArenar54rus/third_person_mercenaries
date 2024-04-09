using UnityEngine;


namespace Module.General.AutoReferences
{
	[CreateAssetMenu(fileName = "ScriptableObjectReferencesContext", menuName = "Tools/AutoReferences/Scriptable Object References Context", order = 0)]
	public partial class ScriptableObjectReferencesContext : ScriptableObject, IContext
	{
		[SerializeField] private SerializedSource[] sources;
		[SerializeField] private SerializedTarget[] targets;


		public void OnEnable()
		{
			ContextHandler.Add(this);
		}


		public void OnDisable()
		{
			ContextHandler.Remove(this);
		}


		public SourceData[] GetSourcesData()
		{
			int count = sources.Length;
			var result = new SourceData[count];
			for (var i = 0; i < count; i++)
			{
				result[i] = sources[i].GetSourceData();
			}

			return result;
		}


		public TargetData[] GetTargetsData()
		{
			int count = targets.Length;
			var result = new TargetData[count];
			for (var i = 0; i < count; i++)
			{
				result[i] = targets[i].GetTargetData();
			}

			return result;
		}
	}
}
