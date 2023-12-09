using UnityEngine;

namespace TakeTop
{
	public class SlowUpdateProc
	{
		public delegate void SlowUpdateDelegate();


		private readonly SlowUpdateDelegate slowUpdate;


		private float updateTime;
		private float updateCurrentTime;


		public float DeltaTime => updateTime - updateCurrentTime;
		public float UpdateTime
		{
			get => updateTime;
			set => updateTime = value;
		}

		public SlowUpdateProc(SlowUpdateDelegate slowUpdate, float updateTime)
		{
			this.slowUpdate = slowUpdate;
			this.updateTime = updateTime;
		}

		public void ProceedOnFixedUpdate(float deltaTime = -1)
		{
			float dt = deltaTime < 0 ? Time.deltaTime : deltaTime;
			if (updateCurrentTime <= 0)
			{
				slowUpdate();
				updateCurrentTime += updateTime;
			}

			updateCurrentTime -= dt;
		}

		public void CallTick(bool resetTimer = true)
		{
			slowUpdate();

			if (resetTimer)
			{
				updateCurrentTime = updateTime;
			}
		}
	}
}
