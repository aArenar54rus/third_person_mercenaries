using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;


namespace Module.General.AutoReferences
{
	internal class InjectModuleArray : InjectModule
	{
		private static readonly MethodInfo copyArrayMethodInfo = typeof(InjectModuleArray).GetMethod(nameof(CopyArray),
			BindingFlags.Static | BindingFlags.NonPublic);


		public override bool TryConvertToSimple(Type complexType, out Type simpleType)
		{
			if (ValidateType(ref complexType))
			{
				simpleType = complexType.GetElementType();
				return true;
			}

			simpleType = default;
			return false;
		}


		public override bool TrySetValueIntoField(object targetObject, FieldInfo fieldInfo, List<object> values)
		{
			if (!TryConvertToSimple(fieldInfo.FieldType, out Type typeArgument))
			{
				return false;
			}

			fieldInfo.SetValue(targetObject, copyArrayMethodInfo
											 .MakeGenericMethod(typeArgument)
											 .Invoke(this, new object[] { values }));
			return true;
		}


		public override bool TrySetValueIntoProperty(object targetObject, PropertyInfo propertyInfo, List<object> values)
		{
			if (!TryConvertToSimple(propertyInfo.PropertyType, out Type typeArgument))
			{
				return false;
			}

			propertyInfo.SetValue(targetObject, copyArrayMethodInfo
												.MakeGenericMethod(typeArgument)
												.Invoke(this, new object[] { values }));
			return true;
		}


		private static bool ValidateType(ref Type type)
		{
			return type.IsArray;
		}


		private static T[] CopyArray<T>(IList values)
		{
			List<T> copiedList = new List<T>();
			for (int i = 0, count = values.Count; i < count; i++)
			{
				if (values[i] is T tValue)
				{
					copiedList.Add(tValue);
				}
			}

			return copiedList.ToArray();
		}
	}
}
