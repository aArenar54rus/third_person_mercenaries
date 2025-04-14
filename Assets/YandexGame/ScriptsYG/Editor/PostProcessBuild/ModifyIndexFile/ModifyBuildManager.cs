using System;
using System.Reflection;
using System.IO;
using UnityEditor;
using YG.Insides;
using System.Text;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace YG.EditorScr.BuildModify
{
    public partial class ModifyBuildManager
    {
        
        private static string BUILD_PATCH;
        private static InfoYG infoYG;
        private static string indexFile;
        private enum CodeType { js, head, body, init, start };

        public static void ModifyIndex(string buildPatch)
        {
            #if UNITY_WEBGL
            infoYG = ConfigYG.GetInfoYG();
            BUILD_PATCH = buildPatch;
            string filePath = Path.Combine(buildPatch, "index.html");
            indexFile = File.ReadAllText(filePath);

            Type type = typeof(ModifyBuildManager);
            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            foreach (MethodInfo method in methods)
            {
                if (method.Name != nameof(ModifyIndex) && method.GetParameters().Length == 0)
                {
                    ModifyBuildManager scrCopy = new ModifyBuildManager();
                    method.Invoke(scrCopy, BindingFlags.Static | BindingFlags.Public, null, null, null);
                }
            }

            File.WriteAllText(filePath, indexFile);
            Debug.Log("Modify build complete");

            #endif
        }

        [MenuItem("Tools/PluginYG/Modify Index", false)]
        public static void ModifyIndex()
        {
            #if UNITY_WEBGL
            string buildPatch = BuildLog.ReadProperty("Build path");

            if (buildPatch != null)
            {
                ModifyIndex(buildPatch);
                Process.Start("explorer.exe", buildPatch.Replace("/", "\\"));
            }
            else
            {
                Debug.LogError("Path not found:\n" + buildPatch);
            }
            #endif
        }

        static void AddIndexCode(string code, CodeType addCodeType)
        {
            #if UNITY_WEBGL
            string commentHelper = "// Additional script modules:";

            if (addCodeType == CodeType.head)
                commentHelper = "<!-- Additional head modules -->";
            else if (addCodeType == CodeType.body)
                commentHelper = "<!-- Additional body modules -->";
            else if (addCodeType == CodeType.init)
                commentHelper = "// Additional init modules";
            else if (addCodeType == CodeType.start)
                commentHelper = "// Additional start modules";

            StringBuilder sb = new StringBuilder(indexFile);
            int insertIndex = sb.ToString().IndexOf(commentHelper);
            if (insertIndex >= 0)
            {
                insertIndex += commentHelper.Length;
                sb.Insert(insertIndex, "\n" + code + "\n");
                indexFile = sb.ToString();
            }
            #endif
        }
    }
}