using System;
using System.Collections.Generic;
using Zenject;


namespace Arenar.Character
{
	public class KnightComponentsInstaller : Installer
	{
		public override void InstallBindings()
		{
			InstallCharactersComponents();
		}
		
		private void InstallCharactersComponents()
		{
			Dictionary<Type, ICharacterComponent> characterComponentsPool = new Dictionary<Type, ICharacterComponent>();
			
			ICharacterAnimationComponent<CharacterAnimationComponent.Animation, CharacterAnimationComponent.AnimationValue> animationComponent = new CharacterAnimationComponent();
			characterComponentsPool.Add(typeof(ICharacterAnimationComponent), animationComponent);
			Container.BindInstance(animationComponent).AsSingle();
			Container.Inject(animationComponent);
			
			ICharacterMovementComponent movementComponent = new CharacterMovementComponent();
			characterComponentsPool.Add(typeof(ICharacterMovementComponent), movementComponent);
			Container.BindInstance(movementComponent).AsSingle();
			Container.Inject(movementComponent);
			
			ICharacterLiveComponent characterLiveComponent = new CharacterLiveComponent();
			characterComponentsPool.Add(typeof(ICharacterLiveComponent), characterLiveComponent);
			Container.BindInstance(characterLiveComponent).AsSingle();
			Container.Inject(characterLiveComponent);
			
			ICharacterAimComponent characterAimComponent = new SimpleCharacterAimComponent();
			characterComponentsPool.Add(typeof(ICharacterAimComponent), characterAimComponent);
			Container.BindInstance(characterAimComponent).AsSingle();
			Container.Inject(characterAimComponent);
			
			ICharacterRayCastComponent characterRayCastComponent = new CharacterRayCastComponent();
			characterComponentsPool.Add(typeof(ICharacterRayCastComponent), characterRayCastComponent);
			Container.BindInstance(characterRayCastComponent).AsSingle();
			Container.Inject(characterRayCastComponent);
			
			Container.BindInstance(characterComponentsPool).AsSingle();
		}
	}
}