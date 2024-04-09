using System;
using System.Collections.Generic;
using System.Reflection;


namespace Module.General.AutoReferences
{
	internal static class AutoReferencesUtility
	{
		private static readonly InjectModule[] modules =
		{
			new InjectModuleArray(),
			new InjectModuleList()
		};
		private static readonly int modulesCount = modules.Length;

		public const BindingFlags FOUND_BINDING_FLAGS = BindingFlags.Instance
														| BindingFlags.Public
														| BindingFlags.NonPublic;


		public static bool TryGetSimpleType(Type complexType, out Type simpleType)
		{
			for (var i = 0; i < modulesCount; i++)
				if (modules[i].TryConvertToSimple(complexType, out simpleType))
					return true;

			if (complexType.IsClass || complexType.IsInterface)
			{
				simpleType = complexType;
				return true;
			}

			simpleType = default;
			return false;
		}


		public static bool TrySetValueIntoField(object targetObject, FieldInfo fieldInfo, List<object> values)
		{
			Type fieldType = fieldInfo.FieldType;

			for (var i = 0; i < modulesCount; i++)
				if (modules[i].TrySetValueIntoField(targetObject, fieldInfo, values))
					return true;

			if (fieldType.IsClass || fieldType.IsInterface)
			{
				fieldInfo.SetValue(targetObject, values.Count > 0 ? values[0] : default);
				return true;
			}

			return false;
		}


		public static bool TrySetValueIntoProperty(object targetObject, PropertyInfo propertyInfo, List<object> values)
		{
			Type propertyType = propertyInfo.PropertyType;

			for (var i = 0; i < modulesCount; i++)
				if (modules[i].TrySetValueIntoProperty(targetObject, propertyInfo, values))
					return true;

			if (propertyType.IsClass || propertyType.IsInterface)
			{
				propertyInfo.SetValue(targetObject, values.Count > 0 ? values[0] : default);
				return true;
			}

			return false;
		}


		#if UNITY_EDITOR
		public static void FindSourcesAndTargets(
				ReferencesMap map,
				Func<Type, ReferencesMap, UnityEngine.Object[]> findObjectsFunction,
				out SerializedSource[] tempSources,
				out SerializedTarget[] tempTargets
			)
		{
			if (map == null)
			{
				CustomDebug.LogError("Map is null!");
				tempTargets = Array.Empty<SerializedTarget>();
				tempSources = Array.Empty<SerializedSource>();
				return;
			}

			map.UpdateReferencesMap();
			var foundSourcesDictionary = new Dictionary<UnityEngine.Object, List<Type>>();
			var foundTargetsDictionary = new Dictionary<UnityEngine.Object, TargetMembersContainer>();
			Type[] usingTypes = map.CashedUsingTypes;
			var k = 0;
			for (var i = 0; i < usingTypes.Length; i++)
			{
				Type usingType = usingTypes[i];
				bool isSource = map.IsSource(usingType);
				bool isTarget = map.IsTarget(usingType);
				UnityEngine.Object[] foundObjects = findObjectsFunction.Invoke(usingType, map);
				for (var p = 0; p < foundObjects.Length; p++)
				{
					UnityEngine.Object foundObject = foundObjects[p];
					if (isSource)
					{
						if (!foundSourcesDictionary.TryGetValue(foundObject, out List<Type> sourceTypes))
						{
							sourceTypes = new List<Type>();
							foundSourcesDictionary.Add(foundObject, sourceTypes);
						}

						if (!sourceTypes.Contains(usingType))
							sourceTypes.Add(usingType);
					}

					if (isTarget
						&& !foundTargetsDictionary.ContainsKey(foundObject)
						&& map.TryGetFieldInfos(foundObject.GetType(), out TargetMembersContainer membersContainer))
					{
						foundTargetsDictionary.Add(foundObject, membersContainer);
					}
				}
			}

			int foundSourcesCount = foundSourcesDictionary.Count;
			tempSources = new SerializedSource[foundSourcesCount];
			foreach (KeyValuePair<UnityEngine.Object, List<Type>> sourcePair in foundSourcesDictionary)
			{
				tempSources[k] = new SerializedSource(sourcePair.Key, sourcePair.Value.ToArray());
				k++;
			}

			k = 0;
			int foundTargetsCount = foundTargetsDictionary.Count;
			tempTargets = new SerializedTarget[foundTargetsCount];
			foreach (KeyValuePair<UnityEngine.Object, TargetMembersContainer> targetPair in foundTargetsDictionary)
			{
				tempTargets[k] = new SerializedTarget(targetPair.Key, targetPair.Value);
				k++;
			}
		}
		#endif
	}
}
