using System;
using System.Collections.Generic;
using UnityEngine;


namespace Module.General
{
	[Obsolete]
	[Serializable]
	public class ScriptableList<T> : List<T>, ISerializationCallbackReceiver where T : ScriptableObject
	{
		[SerializeField] private T[] items;


		public void OnBeforeSerialize()
		{
			items = ToArray();
		}


		public void OnAfterDeserialize()
		{
			Clear();
			AddRange(items);
		}
	}
}
