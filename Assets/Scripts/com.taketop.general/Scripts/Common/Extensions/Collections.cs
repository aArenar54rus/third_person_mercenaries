using System;
using System.Collections;
using System.Linq;


namespace Module.General
{
	public static partial class Extensions
	{
		public static string CollectionToString(this ICollection collection)
		{
			var res = "[";

			foreach (object item in collection)
			{
				res += item + ", ";
			}

			if (collection.Count > 0)
			{
				res = res.Remove(res.Length - 2);
			}

			res += "]";

			return res;
		}


		public static T GetMedian<T>(this T[] values)
		{
			if (values == null || values.Length == 0)
			{
				return default;
			}

			Array.Sort(values);
			return values[values.Length / 2];
		}


		public static float CalculateAverage(this float[] values)
		{
			if (values == null || values.Length == 0)
			{
				return 0f;
			}

			var sum = 0f;
			for (int i = values.Length - 1; i >= 0; i--)
			{
				sum += values[i];
			}

			return sum / values.Length;
		}


		public static float CalculateAverage(this int[] values)
		{
			if (values == null || values.Length == 0)
			{
				return 0;
			}

			var sum = 0f;
			for (int i = values.Length - 1; i >= 0; i--)
			{
				sum += values[i];
			}

			return sum / values.Length;
		}
	}
}
