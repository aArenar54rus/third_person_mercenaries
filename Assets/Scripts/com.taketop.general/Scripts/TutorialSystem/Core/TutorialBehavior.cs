using System;
using System.Collections;
using UnityEngine;


namespace Module.General.TutorialSystem
{
	public class TutorialBehavior : ScriptableObject
	{
		private const string PREFS_KEY = "TutorialBehavior : ";
		[SerializeField] private string id = default;
		[SerializeField] private TutorialStage[] stages = default;

		protected TutorialController tutorialController;
		protected TutorialSequencer tutorialSequencer = default;

		public string ID => id;

		protected virtual bool IsTutorialCompleted
		{
			get => PlayerPrefs.GetInt(PREFS_KEY + ID, 0) > 0;
			set => PlayerPrefs.SetInt(PREFS_KEY + ID, value ? 1 : 0);
		}


		protected internal virtual void Initialize(TutorialController controller)
		{
			tutorialController = controller;
			tutorialSequencer = new TutorialSequencer(this);
			ReloadSequencer();

			OnInitialized();
		}


		protected virtual void ReloadSequencer()
		{
			tutorialSequencer.Clear();
			foreach (TutorialStage stage in stages)
				tutorialSequencer.Add(stage);
		}


		protected virtual void OnInitialized() {}


		public Coroutine Invoke(Action callback, float time)
		{
			return tutorialController.Invoke(callback, time);
		}


		public Coroutine StartCoroutine(IEnumerator routine)
		{
			return tutorialController.StartCoroutine(routine);
		}


		public void StopCoroutine(Coroutine routine)
		{
			if (routine == null)
				return;

			tutorialController.StopCoroutine(routine);
			routine = null;
		}


		protected virtual void Begin()
		{
			if (!tutorialController.TryBeginTutorial(id))
				return;

			tutorialSequencer.OnSequenceDone += Done;
			tutorialSequencer.Play();
		}


		protected virtual void Stop()
		{
			if (!tutorialController.TryStopTutorial())
				return;

			ReloadSequencer();
			tutorialSequencer.OnSequenceDone -= Done;
		}


		protected virtual void Done()
		{
			if (!tutorialController.TryDoneTutorial())
				return;

			tutorialSequencer.OnSequenceDone -= Done;
			IsTutorialCompleted = true;
		}


		protected internal virtual void Update() {}


		protected internal virtual void FixedUpdate() {}
	}
}
