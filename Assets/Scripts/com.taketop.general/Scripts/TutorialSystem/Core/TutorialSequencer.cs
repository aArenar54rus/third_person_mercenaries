using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Module.General.TutorialSystem
{
	public class TutorialSequencer
	{
		protected readonly TutorialBehavior tutorialBehavior;
		protected readonly Queue<TutorialStage> stages;
		protected Coroutine routine;

		public event Action OnSequenceDone = default;


		public TutorialSequencer(TutorialBehavior behavior)
		{
			tutorialBehavior = behavior;
			stages = new Queue<TutorialStage>();
		}


		protected internal void Add(TutorialStage stage)
		{
			stages.Enqueue(stage);
		}


		protected internal void Clear()
		{
			if (routine != null)
				Stop();
			stages.Clear();
		}


		protected internal void Play()
		{
			if (routine != null)
			{
				Debug.LogWarning($"TutorialSequencer for {tutorialBehavior.ID} is already playing!");
				return;
			}

			routine = tutorialBehavior.StartCoroutine(PlayRoutine());
		}


		protected internal void Stop()
		{
			if (routine != null)
			{
				stages.Peek()?.Deinitialize();
				tutorialBehavior.StopCoroutine(routine);
				routine = null;
			}
		}


		protected  IEnumerator PlayRoutine()
		{
			while (stages.Count > 0)
			{
				stages.Peek().Initialize(tutorialBehavior);

				while (stages.Peek().OnUpdate() == TutorialStage.StageState.Running)
				{
					yield return null;
				}

				stages.Peek().Deinitialize();
				stages.Dequeue().Deinitialize();
			}

			OnSequenceDone?.Invoke();
			routine = null;
		}
	}
}
