using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Module.General.TutorialSystem
{
	public class TutorialController : MonoSingleton<TutorialController>
	{
		private TutorialBehavior activeTutorial = null;
		private Dictionary<string, TutorialBehavior> tutorialsMap = default;

		public event Action<string> OnTutorialBreak = default;
		public event Action<string> OnTutorialDone = default;


		protected override void OnLaunch()
		{
			base.OnLaunch();

			tutorialsMap = new Dictionary<string, TutorialBehavior>();
			List<TutorialBehavior> behaviors = TutorialSettings.Instance.Behaviors;

			foreach (TutorialBehavior behavior in behaviors)
			{
				tutorialsMap.Add(behavior.ID, behavior);
				behavior.Initialize(this);
			}
		}


		public bool TryBeginTutorial(string id)
		{
			if (activeTutorial != null)
			{
				Debug.LogError($"Tutorial with id \"{activeTutorial.ID}\" already launched.", this);
				return false;
			}

			if (!tutorialsMap.TryGetValue(id, out var tutorial))
			{
				Debug.LogError($"There is no tutorial with id \"{id}\" in tutorials collection.", this);
				return false;
			}

			activeTutorial = tutorial;
			return true;
		}


		public bool TryStopTutorial()
		{
			if (activeTutorial == null)
			{
				Debug.LogWarning($"No active tutorials.", this);
				return false;
			}

			string id = activeTutorial.ID;
			activeTutorial = null;
			OnTutorialBreak?.Invoke(id);

			return true;
		}


		public bool TryDoneTutorial()
		{
			if (activeTutorial == null)
			{
				Debug.LogWarning($"No active tutorials.", this);
				return false;
			}

			string id = activeTutorial.ID;
			activeTutorial = null;
			OnTutorialDone?.Invoke(id);

			return true;
		}


		private void Update()
		{
			activeTutorial?.Update();
		}


		private void FixedUpdate()
		{
			activeTutorial?.FixedUpdate();
		}


		internal Coroutine Invoke(Action callback, float time)
		{
			return StartCoroutine(DoInvoke(callback, time));
		}


		private IEnumerator DoInvoke(Action callback, float time)
		{
			yield return new WaitForSeconds(time);
			callback?.Invoke();
		}
	}
}
