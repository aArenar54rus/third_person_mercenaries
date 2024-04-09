using UnityEngine;


namespace Module.General
{
	public static partial class Extensions
	{
		public static float Sqr(this float f) =>
			f * f;


		public static float SqrRt(this float f) =>
			Mathf.Sqrt(f);


		public static bool IsApproximatelyZero(this float f) =>
			Mathf.Approximately(0f, f);


		public static bool IsApproximately(this float f, float f2) =>
			Mathf.Approximately(f, f2);
	}
}
