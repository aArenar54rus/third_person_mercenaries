using System;


namespace Module.General
{
	[Serializable]
	public class DataSaveHolderData {}

	[Serializable]
	public abstract class DataSaveHolder<T> where T : DataSaveHolderData, new()
	{
		protected readonly T data;

		protected abstract string PrefsKey { get; }


		public DataSaveHolder()
		{
			data = CustomPlayerPrefs.GetObjectValue<T>(PrefsKey);

			data = data ?? new T();

			SaveData();
		}


		protected void SaveData() =>
			CustomPlayerPrefs.SetObjectValue(data, PrefsKey);
	}
}
