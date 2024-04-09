using System;
using System.Collections.Generic;
using System.Reflection;


namespace Module.General.AutoReferences
{
	public class TargetMembersContainer
	{
		public readonly List<FieldInfo> FieldInfos = new List<FieldInfo>();
		public readonly List<PropertyInfo> PropertyInfos = new List<PropertyInfo>();


		public TargetMembersContainer(IEnumerable<FieldInfo> fieldInfos, IEnumerable<PropertyInfo> propertyInfos)
		{
			FieldInfos.AddRange(fieldInfos);
			PropertyInfos.AddRange(propertyInfos);
		}


		public void AddBaseTypeMembers(
				Type baseType,
				IReadOnlyDictionary<Type, TargetMembersContainer> membersDictionary
			)
		{
			while (baseType != null)
			{
				if (membersDictionary.TryGetValue(baseType, out TargetMembersContainer baseContainer))
				{
					FieldInfos.AddRange(baseContainer.FieldInfos);
					PropertyInfos.AddRange(baseContainer.PropertyInfos);
				}

				baseType = baseType.BaseType;
			}
		}
	}
}
