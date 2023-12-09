using UnityEngine;


namespace TakeTop.JSON
{
	public abstract class JSONSerializer : MonoBehaviour
	{
		public abstract string Serialize(object obj);
		public abstract object Deserialize(string str);
		public abstract T Deserialize<T>(string str);
	}
}
