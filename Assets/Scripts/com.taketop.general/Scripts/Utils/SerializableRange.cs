using System;
using Random = UnityEngine.Random;


namespace Module.General.Utils
{
	[Serializable]
	public struct SerializableRange
	{
		public float Min;
		public float Max;

		public float RandomValue =>
			Random.Range(Min, Max);


		public SerializableRange(float min, float max)
		{
			Min = min;
			Max = max;
		}


		public bool IsContains(float value) =>
			value >= Min && value <= Max;
	}
}
