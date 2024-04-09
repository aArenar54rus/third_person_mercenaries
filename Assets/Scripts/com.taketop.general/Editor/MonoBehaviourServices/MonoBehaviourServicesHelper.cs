using UnityEngine;
using UnityEditor;


namespace Module.General.Editor
{
	public class MonoBehaviourServicesHelper : UnityEditor.Editor
	{
		private const string UPDATE_SERVICE_NAME = "SceneUpdateService";


		[MenuItem("Tools/Create Scene Update Manager")]
		private static void CreateSceneManager()
		{
			var sceneUpdateService = new GameObject(UPDATE_SERVICE_NAME)
				.AddComponent<SceneUpdateService>();
			EditorUtility.SetDirty(sceneUpdateService);
		}
	}
}
