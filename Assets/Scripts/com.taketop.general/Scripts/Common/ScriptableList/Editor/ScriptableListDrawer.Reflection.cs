using System;
using System.Reflection;
using UnityEditorInternal;
using UnityEngine;

namespace Module.General.Editor
{
	public partial class ScriptableListDrawer
	{
		private static readonly MethodInfo createMediatorGenericMethod =
			typeof(ScriptableListDrawer)
				.GetMethod(
					nameof(CreateMediatorGeneric),
					BindingFlags.Static | BindingFlags.NonPublic
				);

		private static readonly MethodInfo getOptimizedGUIBlockMethod =
			typeof(UnityEditor.Editor)
				.GetMethod(
					"GetOptimizedGUIBlock",
					BindingFlags.Instance | BindingFlags.NonPublic
				);

		private static readonly MethodInfo onOptimizedInspectorGUIMethod =
			typeof(UnityEditor.Editor)
				.GetMethod(
					"OnOptimizedInspectorGUI",
					BindingFlags.Instance | BindingFlags.NonPublic
				);

		private static readonly object[] createMethodArguments = new object[3];
		private static readonly object[] onOptimizedInspectorGUIMethodArguments = new object[1];
		private static readonly object[] optimizedGUIBlockMethodArguments = new object[3];


		private static DrawerMediator CreateMediator(
				string label,
				Type type,
				ReorderableList reorderableList,
				ScriptableObject scriptableObject
			)
		{
			createMethodArguments[0] = label;
			createMethodArguments[1] = reorderableList;
			createMethodArguments[2] = scriptableObject;

			object createdMediator = createMediatorGenericMethod
									 .MakeGenericMethod(type)
									 .Invoke(null, createMethodArguments);

			return createdMediator as DrawerMediator;
		}


		private static DrawerMediator CreateMediatorGeneric<T>(
				string label,
				ReorderableList reorderableList,
				ScriptableObject scriptableObject
			) where T : ScriptableObject =>
			new DrawerMediator<T>(label, reorderableList, scriptableObject);


		private static void ValidateGenericType(Type checkedType, out Type genericTypes)
		{
			while (checkedType != null)
			{
				if (checkedType.IsGenericType && checkedType.GetGenericTypeDefinition() == ScriptableListTypeDefinition)
				{
					genericTypes = checkedType.GetGenericArguments()[0];
					return;
				}

				checkedType = checkedType.BaseType;
			}

			genericTypes = null;
		}


		private static bool GetOptimizedGUIBlock(
				UnityEditor.Editor editor,
				bool isDirty,
				bool isVisible,
				out float height
			)
		{
			optimizedGUIBlockMethodArguments[0] = isDirty;
			optimizedGUIBlockMethodArguments[1] = isVisible;
			bool result = (bool)getOptimizedGUIBlockMethod.Invoke(editor, optimizedGUIBlockMethodArguments);
			height = (float)optimizedGUIBlockMethodArguments[2];

			return result;
		}


		private static void OnOptimizedInspectorGUI(UnityEditor.Editor editor, Rect position)
		{
			onOptimizedInspectorGUIMethodArguments[0] = position;
			onOptimizedInspectorGUIMethod.Invoke(editor, onOptimizedInspectorGUIMethodArguments);
		}
	}
}
