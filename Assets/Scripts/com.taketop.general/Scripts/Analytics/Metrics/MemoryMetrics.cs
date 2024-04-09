#if UNITY_2020_2_OR_NEWER
using Unity.Profiling;


namespace Module.General.Analytics.Performance
{
	public class MemoryMetrics : Metrics
	{
		#region Properties

		public CustomProfilerRecorder GcReservedMemory { get; private set; }

		public CustomProfilerRecorder GcUsedMemory { get; private set; }

		public CustomProfilerRecorder TotalReservedMemory { get; private set; }

		public CustomProfilerRecorder TotalUsedMemory { get; private set; }

		public CustomProfilerRecorder TextureCount { get; private set; }

		public CustomProfilerRecorder TextureMemory { get; private set; }

		#endregion



		#region Methods

		public override void SetEnabled(bool value)
		{
			if (value)
			{
				GcReservedMemory = new CustomProfilerRecorder(ProfilerCategory.Memory, "GC Reserved Memory", KEEP_DATA_SECONDS);
				GcUsedMemory = new CustomProfilerRecorder(ProfilerCategory.Memory, "GC Used Memory", KEEP_DATA_SECONDS);
				TotalReservedMemory = new CustomProfilerRecorder(ProfilerCategory.Memory, "Total Reserved Memory", KEEP_DATA_SECONDS);
				TotalUsedMemory = new CustomProfilerRecorder(ProfilerCategory.Memory, "Total Used Memory", KEEP_DATA_SECONDS);
				TextureCount = new CustomProfilerRecorder(ProfilerCategory.Memory, "Texture Count", KEEP_DATA_SECONDS);
				TextureMemory = new CustomProfilerRecorder(ProfilerCategory.Memory, "Texture Memory", KEEP_DATA_SECONDS);
			}
			else
			{
				GcReservedMemory = null;
				GcUsedMemory = null;
				TotalReservedMemory = null;
				TotalUsedMemory = null;
				TextureCount = null;
				TextureMemory = null;
			}

			base.SetEnabled(value);
		}


		public override void Sample()
		{
			GcReservedMemory.SampleValue();
			GcUsedMemory.SampleValue();
			TotalReservedMemory.SampleValue();
			TotalUsedMemory.SampleValue();
			TextureCount.SampleValue();
			TextureMemory.SampleValue();
		}

		#endregion
	}
}
#endif
