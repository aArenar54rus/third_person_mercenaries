using UnityEngine;
using Zenject;

namespace TakeTop.PreferenceSystem
{
	public class PreferenceInstaller : MonoInstaller
	{
		[SerializeField] private PreferenceManager _preferenceManager;
		
		
		public override void InstallBindings()
		{
			Container.Bind<IPreferenceManager>()
					 .To<PreferenceManager>()
					 .FromInstance(_preferenceManager)
					 //.FromComponentInHierarchy()
					 .AsSingle();
			Debug.LogError(123);
        }
	}
}
