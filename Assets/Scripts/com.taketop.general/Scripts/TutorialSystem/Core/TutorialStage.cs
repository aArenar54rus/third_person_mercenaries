using System;
using System.Collections;
using UnityEngine;


namespace Module.General.TutorialSystem
{
	public abstract class TutorialStage : ScriptableObject
	{
		public enum StageState : byte
		{
			None = 0,
			Running = 1,
			Success = 2,
			Failure = 3
		}

		protected TutorialBehavior tutorialBehavior;

		protected StageState Result { get; set; } = StageState.Running;



		public virtual void Initialize(TutorialBehavior tutorialBehavior)
		{
			this.tutorialBehavior = tutorialBehavior;
			Result = StageState.Running;
			OnInitialized();
		}


		protected virtual void OnInitialized() {}


		public virtual void Deinitialize() {}


		public virtual StageState OnUpdate() =>
			Result;


		protected Coroutine Invoke(Action callback, float time)
		{
			return tutorialBehavior.Invoke(callback, time);
		}


		protected Coroutine StartCoroutine(IEnumerator routine)
		{
			return tutorialBehavior.StartCoroutine(routine);
		}


		protected void StopCoroutine(Coroutine routine)
		{
			if (routine == null)
				return;

			tutorialBehavior.StopCoroutine(routine);
			routine = null;
		}
	}
}
