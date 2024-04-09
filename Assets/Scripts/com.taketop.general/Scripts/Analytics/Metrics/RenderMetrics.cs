#if UNITY_2020_2_OR_NEWER
using Unity.Profiling;


namespace Module.General.Analytics.Performance
{
	public class RenderMetrics : Metrics
	{
		#region Properties

		public CustomProfilerRecorder SetPassCalls { get; private set; }

		public CustomProfilerRecorder DrawCalls { get; private set; }

		public CustomProfilerRecorder Vertices { get; private set; }

		public CustomProfilerRecorder Batches { get; private set; }

		public CustomProfilerRecorder ShadowCaster { get; private set; }

		#endregion



		#region Methods

		public override void SetEnabled(bool value)
		{
			if (value)
			{
				SetPassCalls = new CustomProfilerRecorder(ProfilerCategory.Render, "SetPass Calls Count", KEEP_DATA_SECONDS);
				DrawCalls = new CustomProfilerRecorder(ProfilerCategory.Render, "Draw Calls Count", KEEP_DATA_SECONDS);
				Vertices = new CustomProfilerRecorder(ProfilerCategory.Render, "Vertices Count", KEEP_DATA_SECONDS);
				Batches = new CustomProfilerRecorder(ProfilerCategory.Render, "Static Batches Count", KEEP_DATA_SECONDS);
				ShadowCaster = new CustomProfilerRecorder(ProfilerCategory.Render, "Shadow Casters Count", KEEP_DATA_SECONDS);
			}
			else
			{
				SetPassCalls = null;
				DrawCalls = null;
				Vertices = null;
				Batches = null;
				ShadowCaster = null;
			}

			base.SetEnabled(value);
		}


		public override void Sample()
		{
			SetPassCalls.SampleValue();
			DrawCalls.SampleValue();
			Vertices.SampleValue();
			Batches.SampleValue();
			ShadowCaster.SampleValue();
		}

		#endregion
	}
}
#endif
