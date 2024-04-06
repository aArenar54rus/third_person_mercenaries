using System;
using System.Collections.Generic;
using Zenject;


namespace Arenar.Character
{
    public class ShootingGalleryTargetCharacterComponentsInstaller : Installer
    {
        public override void InstallBindings()
        {
            InstallAiBaseLogicStates();
            InstallCharactersComponents();
        }

        private void InstallCharactersComponents()
        {
            Dictionary<Type, ICharacterComponent> characterComponentsPool = new Dictionary<Type, ICharacterComponent>();

            IAiCharacterBaseLogicComponent aiLogicComponent = new SGTargetBaseLogicComponent();
            characterComponentsPool.Add(typeof(IAiCharacterBaseLogicComponent), aiLogicComponent);
            Container.BindInstance(aiLogicComponent).AsSingle();
            Container.Inject(aiLogicComponent);
            
            ICharacterLiveComponent characterLiveComponent = new SGTargetLiveComponent();
            characterComponentsPool.Add(typeof(ICharacterLiveComponent), characterLiveComponent);
            Container.BindInstance(characterLiveComponent).AsSingle();
            Container.Inject(characterLiveComponent);
            
            ICharacterDescriptionComponent characterDescriptionComponent = new RobotTargetCharacterDescriptionComponent();
            characterComponentsPool.Add(typeof(ICharacterDescriptionComponent), characterDescriptionComponent);
            Container.BindInstance(characterDescriptionComponent).AsSingle();
            Container.Inject(characterDescriptionComponent);
            
            ISearchTargetComponent aiSearchTargetComponent = new AiSearchTargetComponent();
            characterComponentsPool.Add(typeof(ISearchTargetComponent), aiSearchTargetComponent);
            Container.BindInstance(aiSearchTargetComponent).AsSingle();
            Container.Inject(aiSearchTargetComponent);

            ICharacterMovementComponent characterMovementComponent = new SGTargetMovementComponent();
            characterComponentsPool.Add(typeof(ICharacterMovementComponent), characterMovementComponent);
            Container.BindInstance(characterMovementComponent).AsSingle();
            Container.Inject(characterMovementComponent);

            ICharacterAggressionComponent characterAggressionComponent = new AiCharacterAggressionComponent();
            characterComponentsPool.Add(typeof(ICharacterAggressionComponent), characterAggressionComponent);
            Container.BindInstance(characterAggressionComponent).AsSingle();
            Container.Inject(characterAggressionComponent);

            /*ICharacterAttackComponent characterAttackComponent = new CharacterAttackComponent();
            characterComponentsPool.Add(typeof(ICharacterAttackComponent), characterAttackComponent);
            Container.BindInstance(characterAttackComponent).AsSingle();
            Container.Inject(characterAttackComponent);*/

            Container.BindInstance(characterComponentsPool).AsSingle();
        }

        private void InstallAiBaseLogicStates()
        {
            AIState[] characterBaseLogicStates =
            {
                Container.Instantiate<SGTargetMoveState>(),
                Container.Instantiate<SGTargetAttackState>(),
                Container.Instantiate<SGTargetSearchTargetState>(),
            };

            Container.BindInstance(characterBaseLogicStates).AsSingle();
        }
    }
}