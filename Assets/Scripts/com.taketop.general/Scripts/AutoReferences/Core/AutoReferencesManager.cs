using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Module.General.AutoReferences
{
	public class AutoReferencesManager : SelfInstancingMono
	{
		private readonly Dictionary<Type, List<object>> sourcesDictionary = new Dictionary<Type, List<object>>();
		private readonly Dictionary<object, TargetMembersContainer> targetsDictionary =
			new Dictionary<object, TargetMembersContainer>();

		[SerializeField] private ScriptableObjectReferencesContext scriptableObjectReferencesContext = default;

		protected override bool ShouldBeCreatedFromPrefab => true;


		protected override void Initialize()
		{
			DontDestroyOnLoad(this);

			ContextHandler.OnAddContext += ContextHandlerAddContext;
			ContextHandler.OnRemoveContext += ContextHandlerRemoveContext;

			foreach (IContext context in ContextHandler.GetCurrentContexts())
			{
				AddSources(context);
				AddTargets(context);
			}

			FillObjects();
		}


		private void OnDestroy()
		{
			ContextHandler.OnAddContext -= ContextHandlerAddContext;
			ContextHandler.OnRemoveContext -= ContextHandlerRemoveContext;
		}


		private void ContextHandlerAddContext(IContext context)
		{
			AddSources(context);
			AddTargets(context);
			FillObjects();
		}


		private void ContextHandlerRemoveContext(IContext context)
		{
			RemoveInjectObject(context);
			RemoveFillingObject(context);
			FillObjects();
		}


		private void AddSources(IContext context)
		{
			SourceData[] sourceObjects = context.GetSourcesData();
			for (int i = 0, count = sourceObjects.Length; i < count; i++)
			{
				SourceData source = sourceObjects[i];
				Type[] sourceObjectTypes = source.SourceObjectTypes;
				for (int k = 0, typesCount = sourceObjectTypes.Length; k < typesCount; k++)
				{
					Type sourceObjectType = sourceObjectTypes[k];
					if (!sourcesDictionary.TryGetValue(sourceObjectType, out List<object> objects))
					{
						objects = new List<object>();
						sourcesDictionary.Add(sourceObjectType, objects);
					}

					objects.Add(source.SourceObject);
				}
			}
		}


		private void RemoveInjectObject(IContext context)
		{
			SourceData[] sourceObjects = context.GetSourcesData();
			for (int i = 0, count = sourceObjects.Length; i < count; i++)
			{
				SourceData source = sourceObjects[i];
				Type[] sourceObjectTypes = source.SourceObjectTypes;
				for (int k = 0, typesCount = sourceObjectTypes.Length; k < typesCount; k++)
				{
					Type sourceObjectType = sourceObjectTypes[k];
					if (!sourcesDictionary.TryGetValue(sourceObjectType, out List<object> objects))
					{
						continue;
					}

					objects.Remove(source.SourceObject);
				}
			}
		}


		private void AddTargets(IContext context)
		{
			foreach (TargetData targetData in context.GetTargetsData())
			{
				targetsDictionary.Add(targetData.TargetObject, targetData.TargetMembers);
			}
		}


		private void RemoveFillingObject(IContext context)
		{
			foreach (TargetData targetData in context.GetTargetsData())
			{
				targetsDictionary.Remove(targetData.TargetObject);
			}
		}


		private void FillObjects()
		{
			foreach (KeyValuePair<object, TargetMembersContainer> targetPair in targetsDictionary)
			{
				object targetObject = targetPair.Key;
				TargetMembersContainer membersContainer = targetPair.Value;

				for (int i = 0, count = membersContainer.FieldInfos.Count; i < count; i++)
				{
					FieldInfo targetFieldInfo = membersContainer.FieldInfos[i];
					TrySetTargetField(targetObject, targetFieldInfo, sourcesDictionary);
				}

				for (int i = 0, count = membersContainer.PropertyInfos.Count; i < count; i++)
				{
					PropertyInfo targetPropertyInfo = membersContainer.PropertyInfos[i];
					TrySetTargetProperty(targetObject, targetPropertyInfo, sourcesDictionary);
				}
			}
		}


		private static bool TrySetTargetField(object targetObject, FieldInfo targetFieldInfo, IDictionary<Type, List<object>> sourcesDictionary)
		{
			if (!AutoReferencesUtility.TryGetSimpleType(targetFieldInfo.FieldType, out Type fieldType))
			{
				// TODO: Description
				throw new Exception();
				return false;
			}

			if (!sourcesDictionary.TryGetValue(fieldType, out List<object> sourcesObjects))
			{
				sourcesObjects = new List<object>();
				sourcesDictionary.Add(fieldType, sourcesObjects);
			}

			if (!AutoReferencesUtility.TrySetValueIntoField(targetObject, targetFieldInfo, sourcesObjects))
			{
				// ToDo Description
				throw new Exception();
				return false;
			}

			return true;
		}


		private static bool TrySetTargetProperty(object targetObject, PropertyInfo targetPropertyInfo, IDictionary<Type, List<object>> sourcesDictionary)
		{
			if (!AutoReferencesUtility.TryGetSimpleType(targetPropertyInfo.PropertyType, out Type propertyType))
			{
				// ToDo Description
				throw new Exception();
				return false;
			}

			if (!sourcesDictionary.TryGetValue(propertyType, out List<object> sourcesObjects))
			{
				sourcesObjects = new List<object>();
				sourcesDictionary.Add(propertyType, sourcesObjects);
			}

			if (!AutoReferencesUtility.TrySetValueIntoProperty(targetObject, targetPropertyInfo, sourcesObjects))
			{
				// ToDo Description
				throw new Exception();
				return false;
			}

			return true;
		}
	}
}
