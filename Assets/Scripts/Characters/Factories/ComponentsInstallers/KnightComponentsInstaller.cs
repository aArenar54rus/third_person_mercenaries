using System;
using System.Collections.Generic;
using Zenject;


namespace Arenar.Character
{
	public class KnightComponentsInstaller : Installer
	{
		public override void InstallBindings()
		{
			InstallAiBaseLogicStates();
			InstallCharactersComponents();
		}
		
		private void InstallCharactersComponents()
		{
			Dictionary<Type, ICharacterComponent> characterComponentsPool = new Dictionary<Type, ICharacterComponent>();
			
			IAiCharacterBaseLogicComponent aiLogicComponent = new AiCharacterBaseLogicComponent();
			characterComponentsPool.Add(typeof(IAiCharacterBaseLogicComponent), aiLogicComponent);
			Container.BindInstance(aiLogicComponent).AsSingle();
			Container.Inject(aiLogicComponent);
			
			ICharacterAnimationComponent<CharacterAnimationComponent.Animation, CharacterAnimationComponent.AnimationValue> animationComponent = new CharacterAnimationComponent();
			characterComponentsPool.Add(typeof(ICharacterAnimationComponent), animationComponent);
			Container.BindInstance(animationComponent).AsSingle();
			Container.Inject(animationComponent);
			
			ICharacterMovementComponent movementComponent = new CharacterNavMeshMovementComponent();
			characterComponentsPool.Add(typeof(ICharacterMovementComponent), movementComponent);
			Container.BindInstance(movementComponent).AsSingle();
			Container.Inject(movementComponent);
			
			ICharacterLiveComponent characterLiveComponent = new EnemyCharacterLiveComponent();
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
			
			ICharacterAggressionComponent characterAggressionComponent = new AiCharacterAggressionWithPlayerReactionComponent();
			characterComponentsPool.Add(typeof(ICharacterAggressionComponent), characterAggressionComponent);
			Container.BindInstance(characterAggressionComponent).AsSingle();
			Container.Inject(characterAggressionComponent);
			
			ICharacterAttackComponent characterAttackComponent = new CharacterAttackComponent();
			characterComponentsPool.Add(typeof(ICharacterAttackComponent), characterAttackComponent);
			Container.BindInstance(characterAttackComponent).AsSingle();
			Container.Inject(characterAttackComponent);
			
			ICharacterDescriptionComponent characterDescriptionComponent = new PuppetCharacterDescriptionComponent();
			characterComponentsPool.Add(typeof(ICharacterDescriptionComponent), characterDescriptionComponent);
			Container.BindInstance(characterDescriptionComponent).AsSingle();
			Container.Inject(characterDescriptionComponent);
			
			IInventoryComponent characterInventoryComponent = new AiInventoryComponent();
			characterComponentsPool.Add(typeof(IInventoryComponent), characterInventoryComponent);
			Container.BindInstance(characterInventoryComponent).AsSingle();
			Container.Inject(characterInventoryComponent);
			
			Container.BindInstance(characterComponentsPool).AsSingle();
		}
		
		private void InstallAiBaseLogicStates()
		{
			AIState[] characterBaseLogicStates =
			{
				Container.Instantiate<SearchTargetAiState>(),
				Container.Instantiate<MoveToTargetByNavMeshAiState>(),
				Container.Instantiate<AttackTargetAiState>(),
			};

			Container.BindInstance(characterBaseLogicStates).AsSingle();
		}
	}
}