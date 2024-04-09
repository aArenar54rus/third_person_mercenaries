using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;


namespace Module.General.Analytics.Performance
{
	public struct ProfilerFrame
	{
		public double FrameTime;
		public double OtherTime;
		public double RenderTime;
		public double UpdateTime;
	}



	public interface IFrameDataProvider
	{
		float AverageFrameTime { get; }
		float LastFrameTime { get; }
		CircularBuffer<ProfilerFrame> FrameBuffer { get; }
	}



	public class PerformanceAnalytics : MonoSingleton<PerformanceAnalytics>
	{
		public static event Action OnUpdateFinished;

		private IFrameDataProvider frameDataProvider;



		#region Properties

		#if UNITY_2020_2_OR_NEWER
		public RenderMetrics RenderMetrics { get; private set; }

		public MemoryMetrics MemoryMetrics { get; private set; }

		public GpuMetrics GPUMetrics { get; private set; }
		#endif

		public FpsMetrics FPSMetrics { get; private set; }

		public CpuMetrics CPUMetrics { get; private set; }

		#endregion



		#region Class Lifecycle

		protected override void Initialize()
		{
			base.Initialize();

			#if UNITY_2020_2_OR_NEWER
			RenderMetrics = new RenderMetrics();
			MemoryMetrics = new MemoryMetrics();
			GPUMetrics = new GpuMetrics();
			#else
			Debug.LogWarning($"<b>{nameof(PerformanceAnalytics)}:</b> Unity version lower 2020.2, some metrics unavailable.");
			#endif

			FPSMetrics = new FpsMetrics();
			CPUMetrics = new CpuMetrics();

			StartCoroutine(SampleMaxDeltaTimesRoutine());

			GetFrameDataProvider();
		}

		#endregion



		#region Methods

		public static void SetActive(bool value) =>
			Instance.gameObject.SetActive(value);


		public IFrameDataProvider GetFrameDataProvider()
		{
			#if UNITY_2018_1_OR_NEWER
			if (GraphicsSettings.renderPipelineAsset != null)
			{
				return frameDataProvider ??= gameObject.AddComponent<RPFrameProfiler>();
			}
			#endif

			return frameDataProvider ??= gameObject.AddComponent<FrameProfiler>();
		}


		public string CollectAllAvailableData()
		{
			var sb = new StringBuilder(500);

			#if UNITY_2020_2_OR_NEWER
			var recorders = new List<CustomProfilerRecorder>
			{
				MemoryMetrics.TextureCount,
				MemoryMetrics.TextureMemory,
				MemoryMetrics.GcReservedMemory,
				MemoryMetrics.GcUsedMemory,
				MemoryMetrics.TotalReservedMemory,
				MemoryMetrics.TotalUsedMemory,
				RenderMetrics.Batches,
				RenderMetrics.Vertices,
				RenderMetrics.DrawCalls,
				RenderMetrics.ShadowCaster,
				RenderMetrics.SetPassCalls,
			};

			foreach (var recorder in recorders)
			{
				sb.AppendLine(recorder.ToString());
			}

			sb.AppendLine(GPUMetrics.GpuTime.ToString());
			#endif

			sb.AppendLine(CPUMetrics.CpuTime.ToString());

			sb.AppendLine(FPSMetrics.Fps.ToString());
			sb.AppendLine(FPSMetrics.MaxDeltaTimes.ToString());
			sb.AppendLine($"AverageFPS: {FPSMetrics.AverageFPS}");
			sb.AppendLine($"FpsMedianSamples: {FPSMetrics.FpsMedianSamples}");
			sb.AppendLine($"FpsMedianTotal: {FPSMetrics.FpsMedianTotal}");

			return sb.ToString();
		}

		#endregion



		#region Unity Lifecycle

		private void Update()
		{
			#if UNITY_2020_2_OR_NEWER
			if (MemoryMetrics.IsValid)
			{
				MemoryMetrics.Sample();
			}

			if (RenderMetrics.IsValid)
			{
				RenderMetrics.Sample();
			}

			if (GPUMetrics.IsValid)
			{
				GPUMetrics.Sample();
			}
			#endif

			if (CPUMetrics.IsValid)
			{
				CPUMetrics.Sample();
			}

			if (FPSMetrics.IsValid)
			{
				FPSMetrics.Sample();
			}

			OnUpdateFinished?.Invoke();
		}

		#endregion



		#region Coroutines

		private IEnumerator SampleMaxDeltaTimesRoutine()
		{
			while (true)
			{
				var maxDeltaTime = 0.0f;

				for (var timer = 1.0f; timer > 0; timer -= Time.deltaTime)
				{
					yield return new WaitForEndOfFrame();

					if (maxDeltaTime < Time.deltaTime)
					{
						maxDeltaTime = Time.deltaTime;
					}
				}

				FPSMetrics.MaxDeltaTimes.SampleValue(maxDeltaTime);
			}
		}

		#endregion
	}
}
