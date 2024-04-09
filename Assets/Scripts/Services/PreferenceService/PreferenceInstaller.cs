using UnityEngine;
using Zenject;

namespace TakeTop.PreferenceSystem
{
	public class PreferenceInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container.Bind<IPreferenceManager>()
					 .To<PreferenceManager>()
					 .FromComponentInHierarchy()
					 .AsSingle();
        }
	}
}
