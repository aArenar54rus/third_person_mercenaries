using System;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
#endif

namespace Module.General
{
	[IncludeMyAttributes]
	[ListDrawerSettings(CustomRemoveElementFunction = "@$property.GetAttribute<NestedScriptableObjectListAttribute>().RemoveObject($removeElement, $property)", Expanded = true)]
	[ValueDropdown("@$property.GetAttribute<NestedScriptableObjectListAttribute>().GetAllObjectsOfType()", FlattenTreeView = true)]
	[OnCollectionChanged("@$property.GetAttribute<NestedScriptableObjectListAttribute>().OnCollectionChange($info)")]

	public class NestedScriptableObjectListAttribute : Attribute
	{
		#if UNITY_EDITOR
		public List<UnityEngine.Object> objectsToRemove = new List<UnityEngine.Object>();
		public List<ScriptableObject> objectsToCreate = new List<ScriptableObject>();

		public Type Type;


		protected void RemoveObject(UnityEngine.Object objectToRemove, InspectorProperty property)
		{
			objectsToRemove.Add(objectToRemove);
		}


		protected IEnumerable GetAllObjectsOfType()
		{
			var items = UnityEditor.AssetDatabase.FindAssets("t:Monoscript", new[] { "Assets/Scripts" })
								   .Select(x => UnityEditor.AssetDatabase.GUIDToAssetPath(x))
								   .Where(x => IsCorrectType(UnityEditor.AssetDatabase.LoadAssetAtPath<MonoScript>(x)))
								   .Select(x => new ValueDropdownItem(System.IO.Path.GetFileName(x), ScriptableObject.CreateInstance(UnityEditor.AssetDatabase.LoadAssetAtPath<MonoScript>(x).GetClass())));
			return items;
		}


		protected bool IsCorrectType(MonoScript script)
		{
			if (script != null && Type != null)
			{
				Type scriptType = script.GetClass();
				if (scriptType != null
					&& (scriptType.Equals(Type) || scriptType.IsSubclassOf(Type)) && !scriptType.IsAbstract)
				{
					return true;
				}
			}

			return false;
		}


		protected void OnCollectionChange(CollectionChangeInfo info)
		{
			if (info.ChangeType == CollectionChangeType.Add)
			{
				objectsToCreate.Add((ScriptableObject)info.Value);
			}
		}
		#endif
	}
}
