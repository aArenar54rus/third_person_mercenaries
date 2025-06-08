using UnityEngine;
using Zenject;

namespace Arenar.PreferenceSystem
{
	public class PreferenceInstaller : MonoInstaller {
		[SerializeField]
		private PreferenceManager _manager;
		
		public override void InstallBindings()
		{
			Container.Bind<IPreferenceManager>()
					.To<PreferenceManager>()
					.FromInstance(_manager)
					.AsSingle();
		}
	}
}
