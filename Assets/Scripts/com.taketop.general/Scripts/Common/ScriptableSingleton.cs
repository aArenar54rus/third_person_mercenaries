using UnityEngine;
#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
#endif


namespace Module.General
{
	public abstract class ScriptableSingleton : ScriptableObject
	{
		#if UNITY_EDITOR
		[InitializeOnLoadMethod]
		private static void InitializeOnLoad()
		{
			Type[] types = typeof(ScriptableSingleton).GetAssignableTypes();

			foreach (Type type in types)
			{
				var singleton = (ScriptableSingleton)CreateInstance(type);
				singleton.Touch();
			}
		}

		protected abstract void Touch();
		#endif
	}



	public abstract class ScriptableSingleton<T> : ScriptableSingleton where T : ScriptableObject
	{
		#region Variables

		private static T cachedInstance;

		#endregion



		#region Properties

		protected static string FileName => typeof(T).Name;
		protected static string DefaultAssetPath => $"Assets/Resources/{FileName}.asset";

		public static T Instance
		{
			get
			{
				if (cachedInstance == null)
				{
					#if UNITY_EDITOR
					string[] guids = AssetDatabase.FindAssets($"t:{FileName}");
					if (guids.Length > 0)
					{
						string path = AssetDatabase.GUIDToAssetPath(guids[0]);
						cachedInstance = AssetDatabase.LoadAssetAtPath<T>(path);
					}
					else
					{
						if (File.Exists(DefaultAssetPath))
						{
							Debug.LogWarning($"Impossible to load <b>{FileName}</b> right now.");
							return null;
						}
					}

					#else
					cachedInstance = Resources.Load(FileName) as T;
					#endif
				}

				if (cachedInstance == null)
				{
					#if UNITY_EDITOR
					cachedInstance = CreateInstance<T>();
					SaveAsset(cachedInstance);
					#else
					CustomDebug.LogWarning($"No instance of {FileName} found, using default values");
					cachedInstance = CreateInstance<T>();
					#endif
				}

				return cachedInstance;
			}
		}

		#endregion



		#region Private methods

		#if UNITY_EDITOR

		protected override sealed void Touch() =>
			_ = Instance;


		protected virtual void OnCreated() {}


		private static void SaveAsset(T obj)
		{
			string dirName = Path.GetDirectoryName(DefaultAssetPath);
			if (!Directory.Exists(dirName))
			{
				Directory.CreateDirectory(dirName);
			}

			AssetDatabase.CreateAsset(obj, DefaultAssetPath);
			(obj as ScriptableSingleton<T>).OnCreated();
			EditorUtility.SetDirty(obj);
			AssetDatabase.SaveAssets();

			CustomDebug.Log($"Saved <b>{FileName}</b> instance", obj);
		}

		#endif

		#endregion
	}
}
