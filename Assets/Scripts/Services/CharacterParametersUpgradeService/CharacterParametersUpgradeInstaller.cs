using Arenar.Character;
using UnityEngine;
using Zenject;


namespace Arenar.Installers
{
    public class CharacterParametersUpgradeInstaller : MonoInstaller
    {
        [SerializeField]
        private CharacterParametersUpgradeData data;
		
		
        public override void InstallBindings()
        {
            Container.Bind<PlayerCharacterSkillUpgradeService>()
                .AsSingle()
                .NonLazy();
            
            Container.BindInstance(data)
                .AsSingle()
                .NonLazy();
        }
    }
}