using System;
using Random = UnityEngine.Random;


namespace Module.General.Utils
{
	[Serializable]
	public struct SerializableRangeInt
	{
		public int Min;
		public int Max;

		public int RandomValue =>
			Random.Range(Min, Max);

		public int RandomValueInclusive =>
			Random.Range(Min, Max + 1);


		public SerializableRangeInt(int min, int max)
		{
			Min = min;
			Max = max;
		}


		public bool IsContains(int value) =>
			value >= Min && value <= Max;
	}
}
