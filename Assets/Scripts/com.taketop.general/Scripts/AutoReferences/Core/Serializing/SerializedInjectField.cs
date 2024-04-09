using System;
using System.Reflection;
using UnityEngine;


namespace Module.General.AutoReferences
{
	[Serializable]
	internal struct SerializedInjectField
	{
		[SerializeField] private SerializedType type;
		[SerializeField] private string path;


		public SerializedInjectField(FieldInfo fieldInfo)
		{
			type = fieldInfo.DeclaringType;
			path = fieldInfo.Name;
		}


		public FieldInfo GetFieldInfo()
		{
			return type.Type.GetField(path, AutoReferencesUtility.FOUND_BINDING_FLAGS);
		}
	}
}
