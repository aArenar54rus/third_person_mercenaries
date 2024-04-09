using UnityEngine;


namespace Module.General.AutoReferences
{
	public partial class SceneReferencesContext : MonoBehaviour, IContext
	{
		[SerializeField] internal SerializedSource[] sources = default;
		[SerializeField] internal SerializedTarget[] targets = default;


		private void Awake()
		{
			#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				return;
			}
			#endif
			ContextHandler.Add(this);
		}


		private void OnDestroy()
		{
			#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				return;
			}
			#endif
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
