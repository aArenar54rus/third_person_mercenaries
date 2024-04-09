using System;
using System.Collections.Generic;
using System.Reflection;


namespace Module.General.AutoReferences
{
	internal abstract class InjectModule
	{
		public abstract bool TryConvertToSimple(Type complexType, out Type simpleType);
		public abstract bool TrySetValueIntoField(object targetObject, FieldInfo fieldInfo, List<object> values);
		public abstract bool TrySetValueIntoProperty(object targetObject, PropertyInfo propertyInfo, List<object> values);
	}
}
