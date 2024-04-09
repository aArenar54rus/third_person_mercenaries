#if UNITY_2020_2_OR_NEWER
using UnityEngine;
using UnityEngine.Profiling;


namespace Module.General.Analytics.Performance
{
	public class GpuMetrics : Metrics
	{
		#region Fields

		public CustomPerformanceRecorder<float> GpuTime = new CustomPerformanceRecorder<float>(KEEP_DATA_SECONDS, "GPU Time");
		private Recorder behaviourUpdateRecorder;

		#endregion



		#region Properties

		public override bool IsValid =>
			IsEnabled && SystemInfo.supportsGpuRecorder && behaviourUpdateRecorder.isValid;

		#endregion



		#region Methods

		public override void SetEnabled(bool value)
		{
			if (value)
			{
				GpuTime = new CustomPerformanceRecorder<float>(KEEP_DATA_SECONDS, "GPU Time");
				behaviourUpdateRecorder = Recorder.Get("BehaviourUpdate");
				behaviourUpdateRecorder.enabled = true;
			}
			else
			{
				GpuTime = null;
				behaviourUpdateRecorder = null;
			}

			base.SetEnabled(value);
		}


		public override void Sample()
		{
			SampleGpuTime();
		}


		private void SampleGpuTime()
		{
			if (!IsValid)
			{
				return;
			}

			float gpuTime = NanosecondsToMilliseconds(behaviourUpdateRecorder.gpuElapsedNanoseconds);

			GpuTime.SampleValue(gpuTime);
		}

		#endregion
	}
}
#endif
