using UnityEngine.Profiling;


namespace Module.General.Analytics.Performance
{
	public class CpuMetrics : Metrics
	{
		#region Fields

		public CustomPerformanceRecorder<float> CpuTime { get; private set; }
		private Recorder behaviourUpdateRecorder;

		#endregion



		#region Properties

		public override bool IsValid =>
			IsEnabled && behaviourUpdateRecorder.isValid;

		#endregion



		#region Methods

		public override void SetEnabled(bool value)
		{
			if (value)
			{
				CpuTime = new CustomPerformanceRecorder<float>(KEEP_DATA_SECONDS, "CPU Time");
				behaviourUpdateRecorder = Recorder.Get("BehaviourUpdate");
				behaviourUpdateRecorder.enabled = true;
			}
			else
			{
				CpuTime = null;
				behaviourUpdateRecorder = null;
			}

			base.SetEnabled(value);
		}


		public override void Sample()
		{
			SampleCpuTime();
		}


		private void SampleCpuTime()
		{
			if (!IsValid)
			{
				return;
			}

			float cpuTime = NanosecondsToMilliseconds(behaviourUpdateRecorder.elapsedNanoseconds);

			CpuTime.SampleValue(cpuTime);
		}

		#endregion
	}
}
