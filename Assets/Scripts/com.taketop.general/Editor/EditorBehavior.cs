using UnityEditor;


namespace Module.General
{
    [InitializeOnLoad]
    public class EditorBehavior
    {
        static EditorBehavior()
        {
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
        }


        private static void OnBeforeAssemblyReload()
        {
            AutoExitPlayMode();
        }


        private static void AutoExitPlayMode()
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }
        }
    }
}
