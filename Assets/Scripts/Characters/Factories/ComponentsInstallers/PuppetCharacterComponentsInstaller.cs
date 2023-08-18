using System;
using System.Collections.Generic;
using Zenject;


namespace Arenar.Character
{
    public class PuppetCharacterComponentsInstaller : Installer
    {
        public override void InstallBindings()
        {
            InstallCharactersComponents();
        }

        private void InstallCharactersComponents()
        {
            Dictionary<Type, ICharacterComponent> characterComponentsPool = new Dictionary<Type, ICharacterComponent>();

            ICharacterLiveComponent characterLiveComponent = new ImmortalCharacterLiveComponent();
            characterComponentsPool.Add(typeof(ICharacterLiveComponent), characterLiveComponent);
            Container.BindInstance(characterLiveComponent).AsSingle();
            Container.Inject(characterLiveComponent);

            ICharacterDescriptionComponent characterDescriptionComponent = new PuppetCharacterDescriptionComponent();
            characterComponentsPool.Add(typeof(ICharacterDescriptionComponent), characterDescriptionComponent);
            Container.BindInstance(characterDescriptionComponent).AsSingle();
            Container.Inject(characterDescriptionComponent);
            
            Container.BindInstance(characterComponentsPool).AsSingle();
        }
    }
}