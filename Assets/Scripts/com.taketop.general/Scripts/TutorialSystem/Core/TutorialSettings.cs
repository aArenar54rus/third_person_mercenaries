using System.Collections.Generic;
using UnityEngine;


namespace Module.General.TutorialSystem
{
	public class TutorialSettings : ScriptableSingleton<TutorialSettings>
	{
		[SerializeField] private List<TutorialBehavior> behaviors = new List<TutorialBehavior>();

		public List<TutorialBehavior> Behaviors => behaviors;
	}
}
