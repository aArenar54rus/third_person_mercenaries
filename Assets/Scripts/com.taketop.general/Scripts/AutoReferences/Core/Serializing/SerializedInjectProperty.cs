using System;
using System.Reflection;
using UnityEngine;


namespace Module.General.AutoReferences
{
	[Serializable]
	internal struct SerializedInjectProperty
	{
		[SerializeField] private SerializedType type;
		[SerializeField] private string path;


		public SerializedInjectProperty(PropertyInfo propertyInfo)
		{
			type = propertyInfo.DeclaringType;
			path = propertyInfo.Name;
		}


		public PropertyInfo GetPropertyInfo()
		{
			return type.Type.GetProperty(path, AutoReferencesUtility.FOUND_BINDING_FLAGS);
		}
	}
}
