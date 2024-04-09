using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace Module.General.Utils
{
	public class RandomObjectPool<T>
	{
		private readonly List<(T, float)> pool = new List<(T, float)>();

		public int Count => pool.Count;



		#region Nested

		public struct Dispersion
		{
			public float chance;
			public float percent;
			public int count;
		}

		#endregion



		#region Ctor

		public RandomObjectPool() {}


		public RandomObjectPool(IEnumerable<(T, float)> table)
		{
			pool.AddRange(table);
		}


		public RandomObjectPool(IDictionary<T, float> table)
		{
			AddRange(table);
		}

		#endregion



		#region Methods

		/// <summary>Add new element to the pool. Chance must be > 0.</summary>
		public void AddItem((T, float) item) =>
			AddItem(item.Item1, item.Item2);


		/// <summary>Add new element to the pool. Chance must be > 0.</summary>
		public void AddItem(T item, float chance)
		{
			if (chance <= 0f)
			{
				Debug.Log($"<b>Element {item} with chance {chance} ignored.</b>\n{GetType().Name}: chance must be > 0.");
				return;
			}

			pool.Add((item, chance));
			pool.Sort((x, y) => x.Item2 <= y.Item2 ? 1 : -1);
		}


		/// <summary>Add new elements to the pool. Chances must be > 0.</summary>
		public void AddRange(IEnumerable<(T, float)> range)
		{
			foreach ((T item, float chance) in range)
				AddItem(item, chance);
		}


		/// <summary>Add new elements to the pool. Chance must be > 0.</summary>
		public void AddRange(IEnumerable<T> range, float chance)
		{
			foreach (var item in range)
				AddItem(item, chance);
		}


		/// <summary>Add new elements to the pool. Chances must be > 0.</summary>
		public void AddRange(IDictionary<T, float> range)
		{
			foreach (KeyValuePair<T, float> item in range)
				AddItem(item.Key, item.Value);
		}


		public void RemoveItem(T item)
		{
			for (int i = 0; i < Count; i++)
				if (pool[i].Item1.Equals(item))
				{
					pool.RemoveAt(i);
					return;
				}
		}


		public void RemoveRange(IEnumerable<T> items)
		{
			var removables = new HashSet<T>(items);
			for (int i = 0; i < Count; i++)
			{
				var item = pool[i].Item1;
				if (removables.Contains(item))
				{
					pool.RemoveAt(i--);
					removables.Remove(item);
				}
			}
		}


		/// <summary>Clear the pool.</summary>
		public void Clear() =>
			pool.Clear();


		/// <summary>Try get a random element.</summary>
		public bool TryTake(out T element)
		{
			element = default;

			if (!TryGetRandomIndex(out int index))
				return false;

			element = pool[index].Item1;

			return true;
		}


		/// <summary>Try get a random element and remove it from rotation.</summary>
		public bool TryPop(out T element)
		{
			element = default;

			if (!TryGetRandomIndex(out int index))
				return false;

			element = pool[index].Item1;
			pool.RemoveAt(index);

			return true;
		}


		public Dictionary<T, Dispersion> GetTestDispersion(int iterations)
		{
			var results = new Dictionary<T, Dispersion>();
			if (Count == 0)
				return results;

			var counter = new Dictionary<T, int>();
			float sum = pool.Sum(item => item.Item2);

			foreach ((T item, float chance) in pool)
			{
				counter.Add(item, 0);
				results.Add(
					item,
					new Dispersion
					{
						chance = chance,
						percent = chance / sum,
					}
				);
			}

			// begin test
			for (var i = 0; i < iterations; i++)
			{
				if (TryTake(out T randomObject))
					counter[randomObject] += 1;
			}

			foreach (T key in counter.Keys)
			{
				Dispersion item = results[key];
				item.count = counter[key];
				results[key] = item;
			}

			return results;
		}


		/// <summary>Get sample of test dispersion of elements in 4 columns:<para />element, given chance, real %, total count</summary>
		public virtual string Test(int iterations)
		{
			Dictionary<T, Dispersion> dispersion = GetTestDispersion(iterations);
			var result = new StringBuilder();

			float sumChance = dispersion.Values.Sum(item => item.chance);
			result.AppendLine($"Test:\t{sumChance}\t100%\t{iterations:###,###,###,###,###,###}");

			foreach (T key in dispersion.Keys)
				result.AppendLine($"{key}\t{dispersion[key].chance}\t{100f*dispersion[key].percent:F2}\t{dispersion[key].count:###,###,###,###,###,###}");

			return result.ToString();
		}


		private bool TryGetRandomIndex(out int index)
		{
			index = -1;

			if (pool.Count == 0)
				return false;

			float max = pool.Sum(item => item.Item2);
			float rnd = Random.Range(0f, max);
			var p = 0f;

			for (var i = 0; i < pool.Count; i++)
			{
				p += pool[i].Item2;
				if (rnd <= p)
				{
					index = i;
					return true;
				}
			}

			return false;
		}

		#endregion
	}
}
