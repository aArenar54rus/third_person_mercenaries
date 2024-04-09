using System;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Module.General.AutoReferences
{
	[Serializable]
	internal class SerializedSource
	{
		[SerializeField] private Object sourceObject = default;
		[SerializeField] private SerializedType[] SourceTypes = default;


		public SerializedSource(Object sourceObject, Type[] sourceTypes)
		{
			this.sourceObject = sourceObject;
			int count = sourceTypes.Length;
			SourceTypes = new SerializedType[count];
			for (var i = 0; i < count; i++)
			{
				SourceTypes[i] = new SerializedType(sourceTypes[i]);
			}
		}


		public SourceData GetSourceData()
		{
			int sourceTypesCount = SourceTypes.Length;
			var sourceTypes = new Type[sourceTypesCount];
			for (var i = 0; i < sourceTypesCount; i++)
			{
				sourceTypes[i] = SourceTypes[i].Type;
			}

			return new SourceData(sourceObject, sourceTypes);
		}
	}
}
