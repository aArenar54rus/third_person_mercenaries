using System;


namespace Module.General.Utils
{
	[Serializable]
	public class SerializablePair<TKey, TValue>
	{
		public TKey key;
		public TValue value;


		public SerializablePair(TKey key, TValue value)
		{
			this.key = key;
			this.value = value;
		}
	}
}
