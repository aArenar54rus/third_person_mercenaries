using UnityEngine;

namespace Arenar.PreferenceSystem
{
	public class SerializationKeyBuilder : MonoBehaviour
	{
		[SerializeField] private string postfix = default;
		[SerializeField] private string prefix = default;

		public string GetSerializationPostfix()
		{
			return postfix;
		}

		public string BuildKey(string incKey)
		{
			return string.Concat(prefix, incKey);
		}
	}
}
