using System;
using System.Collections.Generic;
using Zenject;


namespace Arenar.Character
{
    public class PlayerCharacterComponentsInstaller : Zenject.Installer
    {
        public override void InstallBindings()
        {
            InstallKittyCharactersComponents();
        }

        private void InstallKittyCharactersComponents()
        {
            Dictionary<Type, ICharacterComponent> characterComponentsPool = new Dictionary<Type, ICharacterComponent>();
            
            ICharacterAnimationComponent<CharacterAnimationComponent.KittyAnimation, CharacterAnimationComponent.KittyAnimationValue> animationComponent = new CharacterAnimationComponent();
            characterComponentsPool.Add(typeof(ICharacterAnimationComponent), animationComponent);
            Container.BindInstance(animationComponent).AsSingle();
            Container.Inject(animationComponent);
            
            ICharacterSoundComponent<CharacterSoundComponent.KittySounds> soundComponent = new CharacterSoundComponent();
            characterComponentsPool.Add(typeof(ICharacterSoundComponent), soundComponent);
            Container.BindInstance(soundComponent).AsSingle();
            Container.Inject(soundComponent);

            ICharacterMovementComponent movementComponent = new KittyCharacterMovementComponent();
            characterComponentsPool.Add(typeof(ICharacterMovementComponent), movementComponent);
            Container.BindInstance(movementComponent).AsSingle();
            Container.Inject(movementComponent);
            
/*            ICharacterSkinComponent skinComponent = new KittyCharacterSkinColorComponent();
            characterComponentsPool.Add(typeof(ICharacterSkinColorComponent), skinComponent);
            Container.BindInstance(skinComponent).AsSingle();
            Container.Inject(skinComponent);*/
            
            ICharacterLiveComponent characterLiveComponent = new KittyCharacterLiveComponent();
            characterComponentsPool.Add(typeof(ICharacterLiveComponent), characterLiveComponent);
            Container.BindInstance(characterLiveComponent).AsSingle();
            Container.Inject(characterLiveComponent);
            
            ICharacterInputComponent characterInputComponent = new PlayerCharacterInputComponent();
            characterComponentsPool.Add(typeof(ICharacterInputComponent), characterInputComponent);
            Container.BindInstance(characterInputComponent).AsSingle();
            Container.Inject(characterInputComponent);

            ICharacterRayCastComponent characterRayCastComponent = new CharacterRayCastComponent();
            characterComponentsPool.Add(typeof(ICharacterRayCastComponent), characterRayCastComponent);
            Container.BindInstance(characterRayCastComponent).AsSingle();
            Container.Inject(characterRayCastComponent);
            
/*            ICharacterScaleComponent characterScaleComponent = new KittyCharacterScaleComponent();
            characterComponentsPool.Add(typeof(ICharacterScaleComponent), characterScaleComponent);
            Container.BindInstance(characterScaleComponent).AsSingle();
            Container.Inject(characterScaleComponent);*/
            
            ICharacterPlayerInteractionComponent characterPlayerInteractionComponent = new CharacterPlayerInteractionComponent();
            characterComponentsPool.Add(typeof(ICharacterPlayerInteractionComponent), characterPlayerInteractionComponent);
            Container.BindInstance(characterPlayerInteractionComponent).AsSingle();
            Container.Inject(characterPlayerInteractionComponent);

            Container.BindInstance(characterComponentsPool).AsSingle();
        }
    }
}