using UnityEngine;


namespace Module.General
{
	public abstract class MonoSingleton<T> : SelfInstancingMono where T : SelfInstancingMono
	{
		#region Properties

		public static T Instance { get; private set; }


		protected override bool ShouldCreateSelfInstanceAutomatically => true;

		#endregion



		#region Class Lifecycle

		protected override void Initialize()
		{
			base.Initialize();

			if (Instance != null)
			{
				Debug.LogError($"Singleton {this.GetType().FullName} already exists!");
				Destroy(gameObject);
				return;
			}

			Instance = this as T;

			DontDestroyOnLoad(gameObject);
		}

		#endregion
	}
}
