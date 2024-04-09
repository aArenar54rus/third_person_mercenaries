#if UNITY_2018_1_OR_NEWER

using System.Collections;
using System.Diagnostics;
using UnityEngine;
#if UNITY_2019_3_OR_NEWER
using UnityEngine.Rendering;
#else
using UnityEngine.Experimental.Rendering;
#endif


namespace Module.General.Analytics.Performance
{
	public class RPFrameProfiler : MonoBehaviour, IFrameDataProvider
	{
		private const int FRAME_BUFFER_SIZE = 400;
		private readonly Stopwatch stopwatch = new Stopwatch();

		public float AverageFrameTime { get; private set; }
		public float LastFrameTime { get; private set; }
		public CircularBuffer<ProfilerFrame> FrameBuffer { get; } =
			new CircularBuffer<ProfilerFrame>(FRAME_BUFFER_SIZE);

		// Time between first Update() and last LateUpdate()
		private double updateDuration;

		// Time that render pipeline starts
		private double renderStartTime;

		// Time between scripted render pipeline starts + EndOfFrame
		private double renderDuration;



		private void Awake()
		{
			#if UNITY_2019_3_OR_NEWER
			RenderPipelineManager.beginFrameRendering += RenderPipelineOnBeginFrameRendering;
			#else
            RenderPipeline.beginFrameRendering += RenderPipelineOnBeginFrameRendering;
			#endif

			StartCoroutine(EndOfFrameCoroutine());
		}


		private void Update()
		{
			EndFrame();

			// Set the frame time for the last frame
			if (FrameBuffer.Count > 0)
			{
				ProfilerFrame frame = FrameBuffer.Back();
				frame.FrameTime = Time.unscaledDeltaTime;
				FrameBuffer[FrameBuffer.Count - 1] = frame;
			}

			LastFrameTime = Time.unscaledDeltaTime;

			int frameCount = Mathf.Min(20, FrameBuffer.Count);

			var totalFrameTime = 0d;
			for (var i = 0; i < frameCount; i++)
			{
				totalFrameTime += FrameBuffer[FrameBuffer.Count - 1 - i].FrameTime;
			}

			AverageFrameTime = (float)totalFrameTime / frameCount;

			stopwatch.Start();
		}


		private IEnumerator EndOfFrameCoroutine()
		{
			while (true)
			{
				yield return new WaitForEndOfFrame();

				renderDuration = stopwatch.Elapsed.TotalSeconds - renderStartTime;
			}
		}


		private void PushFrame(double totalTime, double updateTime, double renderTime)
		{
			FrameBuffer.PushBack(new ProfilerFrame
			{
				OtherTime = totalTime - updateTime - renderTime,
				UpdateTime = updateTime,
				RenderTime = renderTime
			});
		}


		private void LateUpdate()
		{
			updateDuration = stopwatch.Elapsed.TotalSeconds;
		}


		#if UNITY_2019_3_OR_NEWER
		private void RenderPipelineOnBeginFrameRendering(
				ScriptableRenderContext context,
				Camera[] cameras
			)
			#else
		private void RenderPipelineOnBeginFrameRendering(Camera[] obj)
			#endif
		{
			renderStartTime = stopwatch.Elapsed.TotalSeconds;
		}


		private void EndFrame()
		{
			if (stopwatch.IsRunning)
			{
				PushFrame(stopwatch.Elapsed.TotalSeconds, updateDuration, renderDuration);

				stopwatch.Reset();
				stopwatch.Start();
			}

			updateDuration = renderDuration = 0;
		}
	}
}
#endif
