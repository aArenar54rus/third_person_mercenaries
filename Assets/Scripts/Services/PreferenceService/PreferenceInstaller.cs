using UnityEngine;
using Zenject;

namespace Arenar.PreferenceSystem
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
