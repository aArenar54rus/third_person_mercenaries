using System;
using Zenject;

namespace TakeTop.PreferenceSystem
{
	public interface IPreferenceManager : ITickable
	{
		void SaveValue<T>(string key, T value);
		void SaveValue<T>(T value);
        void SaveValue(string key, object value);
        object LoadValue(string key, object defaultValue);
		T LoadValue<T>(string key, T defaultValue);
        T LoadValue<T>(T defaultValue);
		T LoadValue<T>() where T : new();
        void EmergencySave();
	}
}
