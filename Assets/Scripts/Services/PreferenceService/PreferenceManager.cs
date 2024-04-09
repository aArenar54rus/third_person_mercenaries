using System;
using System.Collections.Generic;
using System.Threading;
using Module.General;
using TakeTop.JSON;
using TakeTop.MainThread;
using UnityEngine;
using Zenject;

namespace TakeTop.PreferenceSystem
{
	public class PreferenceManager : MonoBehaviour, IPreferenceManager
	{
		[Serializable]
		private class KeySaver
		{
			public List<string> Keys;
		}

		private class WorkingThreadExecuter
		{
			private readonly Thread workingThread;
			private readonly Queue<Action> actions = new Queue<Action>();
			private readonly object syncRoot = new object();
			private MainThreadExecutor mainThreadExecutor;
			private Exception threadException;

			public WorkingThreadExecuter(MainThreadExecutor mainThreadExecutor)
			{
				this.mainThreadExecutor = mainThreadExecutor;
				workingThread = new Thread(ExecuterThread);
				workingThread.Start();
			}

			public void Run(Action action)
			{
				if (null == action)
				{
					Debug.LogError("ExternalThreadExecutor: cannot run null action");

					return;
				}

				lock (syncRoot)
				{
					actions.Enqueue(action);
					Monitor.Pulse(syncRoot);
				}
			}

			private void ExecuterThread()
			{
				Action cachedAction;

				while (true)
				{
					lock (syncRoot)
					{
						if (actions.Count <= 0)
						{
							Monitor.Wait(syncRoot);
						}

						cachedAction = actions.Dequeue();
					}

					try
					{
						cachedAction();
					}
					catch (Exception e)
					{
						threadException = e;
						mainThreadExecutor.Run(ExceptionCallback);
					}
				}
			}

			private void ExceptionCallback()
			{
				Debug.LogError(threadException);
			}
		}


		private readonly Type objectType = typeof(object);
		private readonly Dictionary<string, object> cache = new Dictionary<string, object>();
		private readonly Dictionary<string, object> changes = new Dictionary<string, object>();


		private const string KEYS_LIST_KEY = "KeysList";
		private const float SERIALIZATION_COUNTDOWN = 30f;
		private const float SERIALIZATION_CALL_BIAS = 1f;

		private List<string> cachedKeys = new List<string>(200);
		private WorkingThreadExecuter workingThreadExecuter;
		private SlowUpdateProc serializationKeysUpdateProc;
		private SlowUpdateProc serializationPrefsUpdateProc;

		[SerializeField] private MainThreadExecutor mainThreadExecutor;
		[SerializeField] private JSONSerializer serializer;

		private bool forceSave;
		private bool inited;

        public void SaveValue<T>(string key, T value) =>
            SaveValue(key, (object)value);

        public void SaveValue<T>(T value) =>
            SaveValue(value.GetType().FullName, (object)value);

        public void SaveValue(string key, object value)
        {
            Init();
            if (cache.ContainsKey(key))
            {
                cache[key] = value;
            }
            else
            {
                cache.Add(key, value);
            }

            if (!changes.ContainsKey(key))
            {
                changes.Add(key, value);
            }
            else
            {
                changes[key] = value;
            }

			ForceSaving();
        }

        public object LoadValue(string key, object defaultValue)
        {
            Init();

            if (cache.TryGetValue(key, out object value))
            {
                if (defaultValue == null)
                    return default;

                if (value?.GetType() != defaultValue.GetType())
                {
                    Debug.LogError($"Type of loaded value is not the same "
                                         + $"as default at key"
                                         + $" : {key}; Type {value.GetType()} "
                                         + $"!= {defaultValue.GetType()}");
                    return defaultValue;
                }

                if (value is IConvertible)
                {
                    var convertible = value as IConvertible;
                    return ConvertTo(convertible);
                }
            }

            if (value == null)
            {
                SaveValue(key, defaultValue);
                return defaultValue;
            }

            return value;
        }

        public T LoadValue<T>(string key, T defaultValue) =>
            (T)LoadValue(key, (object)defaultValue);

        public T LoadValue<T>() where T : new() =>
			LoadValue(typeof(T).FullName, new T());

        public T LoadValue<T>(T defaultValue) =>
            LoadValue(defaultValue.GetType().FullName, defaultValue);

        public void EmergencySave()
		{
			Init();
			SerializeKeysInstantly();
			SerializePrefsInstantly();
			forceSave = true;
		}

		public void Tick()
		{
			if (!inited)
			{
				return;
			}

			if (forceSave)
			{
				forceSave = false;
				InnerSave();
			}
			else
			{
				serializationKeysUpdateProc.ProceedOnFixedUpdate();
				serializationPrefsUpdateProc.ProceedOnFixedUpdate();
			}
		}

		[Inject]
		private void Init()
		{
			if (inited)
			{
				return;
			}

			forceSave = false;
			workingThreadExecuter = new WorkingThreadExecuter(mainThreadExecutor);
			PullKeys();
			PullPrefs();
			serializationKeysUpdateProc = new SlowUpdateProc(PushDataParallelKeys,
															 SERIALIZATION_COUNTDOWN);
			serializationPrefsUpdateProc = new SlowUpdateProc(PushDataParallelPrefs,
															  SERIALIZATION_COUNTDOWN
															  + SERIALIZATION_CALL_BIAS);
			inited = true;
		}

		private void InnerSave()
		{
			PlayerPrefs.Save();
		}

		private void PushDataParallelKeys()
		{
			cachedKeys = DuplicateKeys(cache.Keys);
			workingThreadExecuter.Run(() => SerializeKeysParallel(cachedKeys));
		}

		private void PushDataParallelPrefs()
		{
			Dictionary<string, object> cashedChanges = DuplicateChanges(changes);
			workingThreadExecuter.Run(() => SerializePrefsParallel(cashedChanges));
		}

		private void SerializeKeysParallel(List<string> keysList)
		{
			string serializedKeysList = serializer.Serialize(keysList);
			mainThreadExecutor.RunGlobal(() =>
										 {
											 PlayerPrefs.SetString(KEYS_LIST_KEY, serializedKeysList);
											 PlayerPrefs.Save();
										 });
		}

		private void SerializeKeysInstantly()
		{
			List<string> keysList = DuplicateKeys(cache.Keys);
			if (keysList.Count <= 0)
			{
				return;
			}

			string serializedKeysList = serializer.Serialize(keysList);
			PlayerPrefs.SetString(KEYS_LIST_KEY, serializedKeysList);
			PlayerPrefs.Save();
		}

		private void SerializePrefsParallel(object incObject)
		{
			Dictionary<string, object> incomingValues = (Dictionary<string, object>)incObject;
			Dictionary<string, string> serializedValues = SerializePrefs(incomingValues);

			mainThreadExecutor.RunGlobal(() =>
										 {
											 foreach (KeyValuePair<string, string> pair in
												 serializedValues)
											 {
												 PlayerPrefs.SetString(pair.Key, pair.Value);
											 }

											 PlayerPrefs.Save();

											 Dictionary<string, object> duplicatedChanges =
												 DuplicateChanges(changes);
											 foreach (KeyValuePair<string, object> pair in
												 incomingValues)
											 {
												 if (duplicatedChanges.ContainsKey(pair.Key)
													 && (null == pair.Value
														 && null == duplicatedChanges[pair.Key]
														 || duplicatedChanges[pair.Key]
															 .Equals(pair.Value)
														))
												 {
													 changes.Remove(pair.Key);
												 }
											 }
										 });
		}

		private void SerializePrefsInstantly()
		{
			Dictionary<string, string> serializedValues = SerializePrefs(changes);
			if (serializedValues.Count <= 0)
			{
				return;
			}

			foreach (KeyValuePair<string, string> pair in serializedValues)
			{
				PlayerPrefs.SetString(pair.Key, pair.Value);
			}

			PlayerPrefs.Save();

			changes.Clear();
		}

		private Dictionary<string, string> SerializePrefs(Dictionary<string, object> changes)
		{
			Dictionary<string, string> serializedValues = new Dictionary<string, string>();

			foreach (KeyValuePair<string, object> pair in changes)
			{
				string postKey = pair.Key;
				serializedValues.Add(postKey, serializer.Serialize(pair.Value));
			}

			return serializedValues;
		}

		private Dictionary<string, object> DuplicateChanges(Dictionary<string, object> original)
		{
			Dictionary<string, object> duplicate = new Dictionary<string, object>();

			if (original == null
				|| original.Count == 0)
			{
				return duplicate;
			}

			foreach (KeyValuePair<string, object> pair in original)
			{
				duplicate.Add(pair.Key, pair.Value);
			}

			return duplicate;
		}

		private List<string> DuplicateKeys(Dictionary<string, object>.KeyCollection incKeys)
		{
			List<string> duplicate = new List<string>();

			foreach (string key in incKeys)
			{
				duplicate.Add(key);
			}

			return duplicate;
		}

		private void PullKeys()
		{
			string serializedVariable = PlayerPrefs.GetString(KEYS_LIST_KEY);
			List<string> deserializedVariable = null;
			if (!serializedVariable.Equals(String.Empty))
			{
				deserializedVariable = serializer.Deserialize<List<string>>(serializedVariable);
			}

			if (deserializedVariable == null)
			{
				return;
			}

			for (int i = 0; i < deserializedVariable.Count; i++)
			{
				string key = deserializedVariable[i];
				if (!cache.ContainsKey(key))
					cache.Add(key, null);
			}
		}

		private void PullPrefs()
		{
			// Необходимо, чтобы не возникало рассинхронизации (InvalidOperationException: out of sync) <<KrOost.
			List<string> keys = new List<string>();

			foreach (string key in cache.Keys)
			{
				keys.Add(key);
			}

			foreach (string key in keys)
			{
				object value;
				value = PullObject(key);
				cache[key] = value;
			}
		}

		private object PullObject(string key)
		{
			string serializedVariable = PlayerPrefs.GetString(key);
			object deserializedVariable = null;

			if (serializedVariable.Equals(String.Empty))
			{
				return null;
			}

			try
			{
				deserializedVariable = serializer.Deserialize(serializedVariable);
			}
			catch (Exception e)
			{
				#if DEBUG || FORCE_LOGGING
				Debug.LogErrorFormat("PreferenceManager: Can't load value with key {0}", key);
				Debug.LogErrorFormat("PreferenceManager throws: {0}", e);
				#endif
			}

			return deserializedVariable;
		}

        private T ConvertTo<T>(IConvertible obj) =>
            (T)ConvertTo(obj);

        private object ConvertTo(object obj)
        {
            Type t = obj.GetType();
            Type u = Nullable.GetUnderlyingType(t);

            if (u != null)
            {
                return Convert.ChangeType(obj, u);
            }

            return Convert.ChangeType(obj, t);
        }

		private void OnApplicationQuit()
		{
			ForceSaving();
		}

		private void OnApplicationPause(bool pause)
		{
			ForceSaving();
		}

		private void ForceSaving()
		{
			EmergencySave();
			forceSave = false;
			InnerSave();
		}

        public bool IsActive { get; set; }
    }
}
