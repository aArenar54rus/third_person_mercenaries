using UnityEngine;
using Newtonsoft.Json;


namespace Module.General
{
	internal static class CustomPlayerPrefs
	{
		internal static T GetObjectValue<T>(string key)
		{
			if (!PlayerPrefs.HasKey(key))
			{
				return default;
			}

			string jsonString = PlayerPrefs.GetString(key);

			var settings = new JsonSerializerSettings()
			{
				TypeNameHandling = TypeNameHandling.Objects
			};

			return JsonConvert.DeserializeObject<T>(jsonString, settings);
		}


		internal static void SetObjectValue<T>(T obj, string key)
		{
			var settings = new JsonSerializerSettings()
			{
				TypeNameHandling = TypeNameHandling.Objects
			};

			string json = JsonConvert.SerializeObject(obj, settings);

			PlayerPrefs.SetString(key, json);
		}
	}
}
