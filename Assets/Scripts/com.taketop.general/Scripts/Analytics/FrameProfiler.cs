using System.Diagnostics;
using UnityEngine;


namespace Module.General.Analytics.Performance
{
	public class FrameProfiler : MonoBehaviour, IFrameDataProvider
	{
		private const int FRAME_BUFFER_SIZE = 400;
		private const int FRAME_CAP = 20;

		private readonly Stopwatch stopwatch = new Stopwatch();

		// Time between first Update() and last LateUpdate().
		private double updateDuration;

		// Time that first camera rendered.
		private double renderStartTime;

		// Time between first camera prerender and last camera postrender.
		private double renderDuration;
		private int camerasThisFrame;

		public CircularBuffer<ProfilerFrame> FrameBuffer { get; } =
			new CircularBuffer<ProfilerFrame>(FRAME_BUFFER_SIZE);
		public float AverageFrameTime { get; private set; }
		public float LastFrameTime { get; private set; }


		private void Awake()
		{
			Camera.onPreRender += OnCameraPreRender;
			Camera.onPostRender += OnCameraPostRender;
		}


		private void Update()
		{
			camerasThisFrame = 0;

			EndFrame();

			// Set the frame time for the last frame
			if (FrameBuffer.Count > 0)
			{
				ProfilerFrame frame = FrameBuffer.Back();
				frame.FrameTime = Time.unscaledDeltaTime;
				FrameBuffer[FrameBuffer.Count - 1] = frame;
			}

			LastFrameTime = Time.unscaledDeltaTime;

			int frameCount = Mathf.Min(FRAME_CAP, FrameBuffer.Count);

			var totalFrameTime = 0d;
			for (var i = 0; i < frameCount; i++)
			{
				totalFrameTime += FrameBuffer[FrameBuffer.Count - 1 - i].FrameTime;
			}

			AverageFrameTime = (float)totalFrameTime / frameCount;

			stopwatch.Start();
		}


		private void LateUpdate()
		{
			updateDuration = stopwatch.Elapsed.TotalSeconds;
		}


		private void OnCameraPreRender(Camera cam)
		{
			if (camerasThisFrame == 0)
			{
				renderStartTime = stopwatch.Elapsed.TotalSeconds;
			}

			camerasThisFrame++;
		}


		private void OnCameraPostRender(Camera cam)
		{
			renderDuration = stopwatch.Elapsed.TotalSeconds - renderStartTime;
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


		private void PushFrame(double totalTime, double updateTime, double renderTime)
		{
			FrameBuffer.PushBack(new ProfilerFrame
			{
				OtherTime = totalTime - updateTime - renderTime,
				UpdateTime = updateTime,
				RenderTime = renderTime
			});
		}
	}
}
