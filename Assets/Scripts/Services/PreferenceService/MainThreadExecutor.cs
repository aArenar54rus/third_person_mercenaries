using System;
using System.Collections.Concurrent;
using UnityEngine;


namespace TakeTop.MainThread
{
	public class MainThreadExecutor : MonoBehaviour
	{
		private readonly ConcurrentQueue<Action> globalRunnables = new ConcurrentQueue<Action>();
		private readonly ConcurrentQueue<Action> sceneRunnables = new ConcurrentQueue<Action>();

		private const int RUNS_PER_FRAME = 5;

		private bool isResolved;

		public void Run(Action runnable)
		{
			sceneRunnables.Enqueue(runnable);
		}

		public void RunGlobal(Action runnable)
		{
			globalRunnables.Enqueue(runnable);
		}

		public void OnUpdate()
		{
			if (!isResolved)
			{
				return;
			}

			int executedTasksCount = 0;
			while (executedTasksCount < RUNS_PER_FRAME)
			{
				Action action = null;
				if (!sceneRunnables.IsEmpty)
				{
					sceneRunnables.TryDequeue(out action);
				}
				else if (!globalRunnables.IsEmpty)
				{
					globalRunnables.TryDequeue(out action);
				}
				else
				{
					return;
				}

				if (action == null)
				{
					continue;
				}

				executedTasksCount++;
				try
				{
					action.Invoke();
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}
			}
		}
	}
}
