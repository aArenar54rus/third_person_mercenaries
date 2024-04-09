using System;
using System.Reflection;
using UnityEngine;


namespace Module.General.AutoReferences
{
	[Serializable]
	internal class SerializedTargetInfo
	{
		[SerializeField] private SerializedType targetObjectType = default;
		[SerializeField] private string[] fieldsPaths = default;
		[SerializeField] private string[] propertiesPaths = default;


		public SerializedTargetInfo(Type targetObjectType, FieldInfo[] injectFields, PropertyInfo[] injectProperties)
		{
			this.targetObjectType = targetObjectType;
			fieldsPaths = SerializedTypeConverter.GetPaths(injectFields);
			propertiesPaths = SerializedTypeConverter.GetPaths(injectProperties);
		}


		public TargetInfo GetTargetInfo()
		{
			FieldInfo[] fields = SerializedTypeConverter.GetFieldInfos(targetObjectType, fieldsPaths);
			PropertyInfo[] properties = SerializedTypeConverter.GetPropertyInfos(targetObjectType, propertiesPaths);
			return new TargetInfo(targetObjectType, fields, properties);
		}
	}
}
