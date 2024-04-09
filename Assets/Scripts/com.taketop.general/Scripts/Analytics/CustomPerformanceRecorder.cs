using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;


namespace Module.General.Analytics.Performance
{
	public class CustomPerformanceRecorder<T> : Queue<T>
	{
		public readonly string Name;
		public readonly int Capacity;

		public virtual T CurrentValue { get; protected set; }


		public CustomPerformanceRecorder(int capacity, string name = default) : base(capacity)
		{
			Capacity = capacity;
			Name = name;
		}


		public virtual void SampleValue(T value = default(T))
		{
			CurrentValue = value;

			if (Count >= Capacity)
			{
				Dequeue();
			}

			Enqueue(value);
		}


		public override string ToString() =>
			Name + (string.IsNullOrEmpty(Name) ? "" : ": ") + this.CollectionToString();
	}



	#if UNITY_2020_2_OR_NEWER
	public class CustomProfilerRecorder : CustomPerformanceRecorder<long>
	{
		public readonly ProfilerRecorder recorder;

		public override long CurrentValue => recorder.LastValue;


		public CustomProfilerRecorder(
				ProfilerCategory category,
				string statName,
				int capacity,
				ProfilerRecorderOptions options = ProfilerRecorderOptions.Default
			) : base(capacity, statName)
		{
			recorder = ProfilerRecorder.StartNew(category, statName, capacity, options);
		}


		public override void SampleValue(long value = default(long))
		{
			if (value != default(long))
			{
				Debug.LogWarning("You don't need to pass the value to SampleValue() of CustomProfilerRecorder.");
			}

			base.SampleValue(recorder.LastValue);
		}
	}
	#endif
}
