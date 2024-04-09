using System;
using System.Reflection;


namespace Module.General.AutoReferences
{
	internal class TargetInfo
	{
		public readonly Type TargetType;
		public readonly FieldInfo[] FieldInfos;
		public readonly PropertyInfo[] PropertyInfos;


		public TargetInfo(Type type, FieldInfo[] fieldInfos, PropertyInfo[] propertyInfos)
		{
			TargetType = type;
			FieldInfos = fieldInfos;
			PropertyInfos = propertyInfos;
		}


		public TargetMembersContainer GetMembers()
		{
			return new TargetMembersContainer(FieldInfos, PropertyInfos);
		}
	}
}
