using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Module.General.Analytics.Performance
{
	public class FpsMetrics : Metrics
	{
		#region Fields

		public CustomPerformanceRecorder<int> Fps { get; private set; }
		public CustomPerformanceRecorder<float> MaxDeltaTimes { get; private set; }

		private AverageCounter averageFPS = new AverageCounter();

		private Dictionary<int, int> fpsStat = new Dictionary<int, int>();

		#endregion



		#region Properties

		public float AverageFPS => averageFPS;

		public int FpsMedianSamples => Fps.ToArray().GetMedian();

		public int FpsMedianTotal {
			get
			{
				float halfSum = fpsStat.Values.Sum() / 2f;
				int[] ordered =  fpsStat.Keys.ToArray();
				Array.Sort(ordered);
				int p = 0;
				foreach (var key in ordered)
				{
					p += fpsStat[key];
					if (p >= halfSum) return key;
				}

				return 0;
			}
		}

		#endregion



		#region Methods

		public override void SetEnabled(bool value)
		{
			if (value)
			{
				Fps = new CustomPerformanceRecorder<int>(KEEP_DATA_SECONDS, "FPS");
				MaxDeltaTimes = new CustomPerformanceRecorder<float>(KEEP_DATA_SECONDS, "Max Delta Times");
				averageFPS = new AverageCounter();
				fpsStat = new Dictionary<int, int>();
			}
			else
			{
				Fps = null;
				MaxDeltaTimes = null;
				averageFPS = null;
				fpsStat = null;
			}

			base.SetEnabled(value);
		}


		public override void Sample()
		{
			SampleFps();
		}


		private void SampleFps()
		{
			var fpsValue = (int)(1.0f / Time.deltaTime);
			Fps.SampleValue(fpsValue);
			averageFPS.Increase(fpsValue);

			if (!fpsStat.ContainsKey(fpsValue))
				fpsStat.Add(fpsValue, 0);

			fpsStat[fpsValue] += 1;
		}

		#endregion
	}
}
