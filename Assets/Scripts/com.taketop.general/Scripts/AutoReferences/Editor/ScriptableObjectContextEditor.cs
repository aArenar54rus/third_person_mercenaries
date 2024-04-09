#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace Module.General.AutoReferences
{
	[CustomEditor(typeof(ScriptableObjectReferencesContext))]
	public class ScriptableObjectContextEditor : UnityEditor.Editor
	{
		private const float PADDING_BUTTON = 5f;
		private const string FIND_SO_BUTTON = "Find ScrptableObjects";
		private const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.NonPublic;
		private const string METHOD_NAME = nameof(ScriptableObjectReferencesContext.FindScriptableObjects);

		private ScriptableObjectReferencesContext referencesContext;


		public override void OnInspectorGUI()
		{
			if (GUILayout.Button(FIND_SO_BUTTON))
			{
				MethodInfo methodInfo = typeof(ScriptableObjectReferencesContext).GetMethod(METHOD_NAME, BINDING_FLAGS);
				methodInfo?.Invoke(referencesContext, new object[] {});
			}

			GUILayout.Space(PADDING_BUTTON);
			base.OnInspectorGUI();
		}

		private void OnEnable()
		{
			if (target is ScriptableObjectReferencesContext soContext)
			{
				referencesContext = soContext;
			}
		}


		private void OnDisable()
		{
			referencesContext = null;
		}
	}
}
#endif
