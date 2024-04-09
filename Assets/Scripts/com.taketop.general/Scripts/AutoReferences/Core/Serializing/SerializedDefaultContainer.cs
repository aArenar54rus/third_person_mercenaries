using System;
using UnityEngine;


namespace Module.General.AutoReferences
{
	[Serializable]
	internal class SerializedDefaultContainer
	{
		[SerializeField] private SerializedType defaultInterface = default;
		[SerializeField] private SerializedType defaultType = default;

		public SerializedType GetInterface => defaultInterface;
		public SerializedType GetType => defaultType;


		public SerializedDefaultContainer(SerializedType key, SerializedType value)
		{
			defaultInterface = key;
			defaultType = value;
		}
	}
}
