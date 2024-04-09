using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Module.General.AutoReferences
{
	[Serializable]
	internal class SerializedTarget
	{
		[SerializeField] private Object targetObject = default;
		[SerializeField] private string[] fieldPaths = default;
		[SerializeField] private string[] propertyPaths = default;


		public SerializedTarget(Object targetObject, TargetMembersContainer membersContainer)
		{
			this.targetObject = targetObject;
			fieldPaths = SerializedTypeConverter.GetPaths(membersContainer.FieldInfos);
			propertyPaths = SerializedTypeConverter.GetPaths(membersContainer.PropertyInfos);
		}


		public TargetData GetTargetData()
		{
			Type fillingType = targetObject.GetType();
			FieldInfo[] fields = SerializedTypeConverter.GetFieldInfos(fillingType, fieldPaths);
			PropertyInfo[] properties = SerializedTypeConverter.GetPropertyInfos(fillingType, propertyPaths);
			var membersContainer = new TargetMembersContainer(fields, properties);

			return new TargetData(targetObject, membersContainer);
		}
	}
}
