#if UNITY_EDITOR
using System.Reflection;
using UnityEngine;


namespace Module.General.AutoReferences
{
	[UnityEditor.CustomEditor(typeof(SceneReferencesContext))]
	public class SceneContextEditor : UnityEditor.Editor
	{
		private const BindingFlags BINDING_FLAGS = BindingFlags.NonPublic | BindingFlags.Instance;
		private const string FIND_BUTTON_TEXT = "Find MonoBehaviours for AutoReferences System";
		private const string INVOKE_METHOD_NAME = nameof(SceneReferencesContext.FindMonoBehavioursForAutoReferencesSystem);
		private const float PADDING_BUTTON_TOP = 5f;


		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			GUILayout.Space(PADDING_BUTTON_TOP);
			if (GUILayout.Button(FIND_BUTTON_TEXT))
			{
				InvokeSceneContext();
			}
		}


		private void InvokeSceneContext()
		{
			var sceneReferencesContext = (SceneReferencesContext)target;
			ReflectionInvokeMethod<SceneReferencesContext>(INVOKE_METHOD_NAME, sceneReferencesContext, new object[] {});
		}


		private static void ReflectionInvokeMethod<TObject>(string methodName, object instance, params object[] arg)
		{
			MethodInfo methodInfo = ReflectionGetMethod<TObject>(methodName);
			methodInfo?.Invoke(instance, arg);
		}


		private static MethodInfo ReflectionGetMethod<TObject>(string methodName)
		{
			return typeof(TObject).GetMethod(methodName, BINDING_FLAGS);
		}
	}
}
#endif
