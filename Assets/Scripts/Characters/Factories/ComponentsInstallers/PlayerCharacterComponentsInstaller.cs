using System;
using System.Collections.Generic;
using Zenject;


namespace Arenar.Character
{
    public class PlayerCharacterComponentsInstaller : Installer
    {
        public override void InstallBindings()
        {
            InstallCharactersComponents();
        }

        private void InstallCharactersComponents()
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

            ICharacterMovementComponent movementComponent = new CharacterMovementComponent();
            characterComponentsPool.Add(typeof(ICharacterMovementComponent), movementComponent);
            Container.BindInstance(movementComponent).AsSingle();
            Container.Inject(movementComponent);
            
            ICharacterSkinComponent skinComponent = new CharacterSkinComponent();
            characterComponentsPool.Add(typeof(ICharacterSkinComponent), skinComponent);
            Container.BindInstance(skinComponent).AsSingle();
            Container.Inject(skinComponent);
            
            ICharacterLiveComponent characterLiveComponent = new CharacterLiveComponent();
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
            
            ICharacterAimComponent characterAimComponent = new PlayerCharacterAimComponent();
            characterComponentsPool.Add(typeof(ICharacterAimComponent), characterAimComponent);
            Container.BindInstance(characterAimComponent).AsSingle();
            Container.Inject(characterAimComponent);

            ICharacterPlayerInteractionComponent characterPlayerInteractionComponent = new CharacterPlayerInteractionComponent();
            characterComponentsPool.Add(typeof(ICharacterPlayerInteractionComponent), characterPlayerInteractionComponent);
            Container.BindInstance(characterPlayerInteractionComponent).AsSingle();
            Container.Inject(characterPlayerInteractionComponent);
            
            ICharacterAttackComponent characterAttackComponent = new CharacterAttackComponent();
            characterComponentsPool.Add(typeof(ICharacterAttackComponent), characterAttackComponent);
            Container.BindInstance(characterAttackComponent).AsSingle();
            Container.Inject(characterAttackComponent);
            
            ICharacterProgressionComponent characterProgressionComponent = new PlayerCharacterProgressionComponent();
            characterComponentsPool.Add(typeof(ICharacterProgressionComponent), characterProgressionComponent);
            Container.BindInstance(characterProgressionComponent).AsSingle();
            Container.Inject(characterProgressionComponent);

            Container.BindInstance(characterComponentsPool).AsSingle();
        }
    }
}