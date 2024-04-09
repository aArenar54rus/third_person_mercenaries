using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Module.General.AutoReferences
{
	[CreateAssetMenu(fileName = "ReferencesMap", menuName = "Tools/AutoReferences/ReferencesMap", order = 0)]
	public class ReferencesMap : ScriptableObject
	{
		[SerializeField] private SerializedType[] usingTypes;
		[SerializeField] private SerializedType[] sourcesTypes;
		[SerializeField] private SerializedTargetInfo[] targetsInfos;
		[SerializeField] private SerializedDefaultContainer[] defaultContainers;

		[NonSerialized] private Type[] cashedUsingTypes;
		[NonSerialized] private HashSet<Type> cashedSourcesTypesHash;
		[NonSerialized] private HashSet<Type> cashedTargetsTypesHash;
		[NonSerialized] private Dictionary<Type, TargetMembersContainer> cashedTargetsDictionary;

		public Type[] CashedUsingTypes
		{
			get
			{
				if (cashedUsingTypes == null)
				{
					int count = usingTypes.Length;
					cashedUsingTypes = new Type[count];
					for (var i = 0; i < count; i++)
					{
						cashedUsingTypes[i] = usingTypes[i].Type;
					}
				}

				return cashedUsingTypes;
			}
		}


		public Dictionary<SerializedType, SerializedType> DefaultContainers
		{
			get
			{
				var temp = new Dictionary<SerializedType, SerializedType>();
				for (int i = 0, count = defaultContainers.Length; i < count; i++)
				{
					SerializedDefaultContainer defaultContainer = defaultContainers[i];
					temp.Add(defaultContainer.GetInterface, defaultContainer.GetType);
				}

				return temp;
			}
		}

		private Dictionary<Type, TargetMembersContainer> CashedTargetsDictionary
		{
			get
			{
				if (cashedTargetsDictionary == null)
				{
					CalculateFillingDictionary(targetsInfos, out cashedTargetsDictionary);
				}

				return cashedTargetsDictionary;
			}
		}


		private HashSet<Type> CashedSourcesTypesHash
		{
			get
			{
				if (cashedSourcesTypesHash == null)
				{
					cashedSourcesTypesHash = new HashSet<Type>();
					for (int i = 0, count = sourcesTypes.Length; i < count; i++)
					{
						cashedSourcesTypesHash.Add(sourcesTypes[i].Type);
					}
				}

				return cashedSourcesTypesHash;
			}
		}

		private HashSet<Type> CashedTargetsTypesHash
		{
			get
			{
				if (cashedTargetsTypesHash == null)
				{
					cashedTargetsTypesHash = CalculateFillingTypeHash(CashedTargetsDictionary);
				}

				return cashedTargetsTypesHash;
			}
		}


		public bool TryGetFieldInfos(Type fillingType, out TargetMembersContainer membersContainer)
		{
			return CashedTargetsDictionary.TryGetValue(fillingType, out membersContainer);
		}


		public bool IsTarget(Type type)
		{
			return CashedTargetsTypesHash.Contains(type);
		}


		public bool IsSource(Type type)
		{
			return CashedSourcesTypesHash.Contains(type);
		}


		private static void CalculateFillingDictionary(IReadOnlyList<SerializedTargetInfo> targetsInfos, out Dictionary<Type, TargetMembersContainer> membersDictionary)
		{
			membersDictionary = new Dictionary<Type, TargetMembersContainer>();
			for (int i = 0, count = targetsInfos.Count; i < count; i++)
			{
				TargetInfo targetInfo = targetsInfos[i].GetTargetInfo();
				membersDictionary.Add(targetInfo.TargetType, targetInfo.GetMembers());
			}

			foreach (KeyValuePair<Type, TargetMembersContainer> pair in membersDictionary)
			{
				TargetMembersContainer container = pair.Value;
				container.AddBaseTypeMembers(pair.Key.BaseType, membersDictionary);
			}
		}


		private static HashSet<Type> CalculateFillingTypeHash(Dictionary<Type, TargetMembersContainer> fillingDictionary)
		{
			var fillingHash = new HashSet<Type>();
			foreach (Type fillingType in fillingDictionary.Keys)
			{
				fillingHash.Add(fillingType);
			}

			return fillingHash;
		}


		#if UNITY_EDITOR
		[ContextMenu("Update References Map")]
		public void UpdateReferencesMap()
		{
			CalculateTargetsInfos(
				out List<TargetInfo> tempTargetsInfos,
				out List<SerializedDefaultContainer> tempDefaultContainers
			);

			// ToDo Next method exist error.
			CalculateMapCollections(
				tempTargetsInfos,
				out List<Type> tempSourcesTypes,
				out List<Type> tempUsingTypes,
				out SerializedTargetInfo[] newTargetsInfos
			);

			targetsInfos = newTargetsInfos;
			defaultContainers = tempDefaultContainers.ToArray();

			int sourcesTypesCount = tempSourcesTypes.Count;
			sourcesTypes = new SerializedType[sourcesTypesCount];
			for (var i = 0; i < sourcesTypesCount; i++)
			{
				sourcesTypes[i] = new SerializedType(tempSourcesTypes[i]);
			}

			int usingTypesCount = tempUsingTypes.Count;
			usingTypes = new SerializedType[usingTypesCount];
			for (var i = 0; i < usingTypesCount; i++)
			{
				usingTypes[i] = new SerializedType(tempUsingTypes[i]);
			}

			cashedUsingTypes = null;
			cashedSourcesTypesHash = null;
			cashedTargetsTypesHash = null;
			cashedTargetsDictionary = null;
			EditorUtility.SetDirty(this);
			// AssetDatabase.SaveAssets();
		}


		private static void CalculateTargetsInfos(out List<TargetInfo> targetsInfos, out List<SerializedDefaultContainer> defaultContainers)
		{
			targetsInfos = new List<TargetInfo>();

			defaultContainers = new List<SerializedDefaultContainer>();
			var fields = new List<FieldInfo>();
			var properties = new List<PropertyInfo>();

			Type[] allTypes = Assembly.GetExecutingAssembly().GetTypes();

			foreach (Type type in allTypes)
			{
				if (!type.IsClass || type.IsGenericType || type.IsAbstract)
				{
					continue;
				}

				FindContainers(type, defaultContainers);

				fields.Clear();
				properties.Clear();
				FindFields(type, fields);
				FindProperties(type, properties);

				if (fields.Count > 0 || properties.Count > 0)
				{
					targetsInfos.Add(new TargetInfo(type, fields.ToArray(), properties.ToArray()));
				}
			}
		}


		private static void FindContainers(Type type, List<SerializedDefaultContainer> injectContainers)
		{
			var injectContainerAttribute = type.GetCustomAttribute<DefaultReferenceAttribute>(true);
			if (injectContainerAttribute == null)
			{
				return;
			}

			Type[] interfaces = type.GetInterfaces();
			for (int i = 0, interfacesCount = interfaces.Length; i < interfacesCount; i++)
			{
				var found = false;
				Type foundInterface = interfaces[i];
				for (int k = 0, containersCount = injectContainers.Count; k < containersCount; k++)
				{
					SerializedDefaultContainer defaultContainer = injectContainers[i];
					if (defaultContainer.GetInterface.Type == foundInterface)
					{
						found = true;
						break;
					}
				}

				if (found == false)
				{
					injectContainers.Add(new SerializedDefaultContainer(foundInterface, type));
				}
			}
		}


		private static void FindFields(Type type, List<FieldInfo> fieldsList)
		{
			while (type != null)
			{
				FieldInfo[] fields = type.GetFields(AutoReferencesUtility.FOUND_BINDING_FLAGS);
				for (var i = 0; i < fields.Length; i++)
				{
					FieldInfo field = fields[i];
					var fieldAttribute = field.GetCustomAttribute<FindReferenceAttribute>(true);
					if (fieldAttribute != null && !ContainsName(fieldsList, field))
					{
						fieldsList.Add(field);
					}
				}

				type = type.BaseType;
			}
		}


		private static void FindProperties(Type type, List<PropertyInfo> propertyList)
		{
			while (type != null)
			{
				PropertyInfo[] properties = type.GetProperties(AutoReferencesUtility.FOUND_BINDING_FLAGS);
				for (var i = 0; i < properties.Length; i++)
				{
					PropertyInfo propertyInfo = properties[i];
					var propertyAttribute = propertyInfo.GetCustomAttribute<FindReferenceAttribute>(true);
					if (propertyAttribute != null && !ContainsName(propertyList, propertyInfo))
					{
						propertyList.Add(propertyInfo);
					}
				}

				type = type.BaseType;
			}
		}


		private static void CalculateMapCollections(List<TargetInfo> targetsInfos, out List<Type> membersTypes,
													out List<Type> usingTypes, out SerializedTargetInfo[] targetsArray)
		{
			int targetsCount = targetsInfos.Count;
			targetsArray = new SerializedTargetInfo[targetsCount];
			membersTypes = new List<Type>();
			usingTypes = new List<Type>();
			for (var i = 0; i < targetsCount; i++)
			{
				FieldInfo[] fieldInfos = targetsInfos[i].FieldInfos;
				PropertyInfo[] propertyInfos = targetsInfos[i].PropertyInfos;
				Type targetType = targetsInfos[i].TargetType;
				targetsArray[i] = new SerializedTargetInfo(targetType, fieldInfos, propertyInfos);

				if (!usingTypes.Contains(targetType))
				{
					usingTypes.Add(targetType);
				}

				for (var k = 0; k < fieldInfos.Length; k++)
				{
					CheckType(fieldInfos[k].FieldType, membersTypes, usingTypes);
				}

				for (var k = 0; k < propertyInfos.Length; k++)
				{
					CheckType(propertyInfos[k].PropertyType, membersTypes, usingTypes);
				}
			}
		}


		private static void CheckType(Type complexType, ICollection<Type> membersTypes, ICollection<Type> usingTypes)
		{
			if (!AutoReferencesUtility.TryGetSimpleType(complexType, out Type fieldType))
			{
				CustomDebug.LogError($"AutoReferences can't get simple <b>Type</b> from {complexType}!");
				return;
			}

			if (!membersTypes.Contains(fieldType))
			{
				membersTypes.Add(fieldType);
			}

			if (!usingTypes.Contains(fieldType))
			{
				usingTypes.Add(fieldType);
			}
		}


		private static bool ContainsName<T>(IReadOnlyList<T> fieldsList, T fieldInfo) where T : MemberInfo
		{
			string fieldName = fieldInfo.Name;
			for (int i = 0, count = fieldsList.Count; i < count; i++)
			{
				if (fieldsList[i].Name == fieldName)
				{
					return true;
				}
			}

			return false;
		}
		#endif
	}
}
