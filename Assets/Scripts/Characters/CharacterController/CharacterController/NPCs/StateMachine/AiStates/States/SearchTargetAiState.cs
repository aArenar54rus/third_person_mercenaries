using UnityEngine;


namespace Arenar.Character
{
    public class SearchTargetAiState : AIState
    {
        private ICharacterLiveComponent characterLiveComponent;
        private ICharacterMovementComponent characterMovementComponent;
        private ICharacterAggressionComponent characterAggressionComponent;
        
        
        public override void Initialize(ICharacterEntity character)
        {
            base.Initialize(character);
            
            character.TryGetCharacterComponent(out characterLiveComponent);
            character.TryGetCharacterComponent(out characterMovementComponent);
            character.TryGetCharacterComponent(out characterAggressionComponent);
        }

        public override void DeInitialize()
        {
            characterLiveComponent.OnCharacterGetDamageBy -= OnCharacterGetDamageBy;
        }
        
        public override void OnStateSyncUpdate()
        {
            characterMovementComponent.Move(Vector2.zero, false);
            
            if (characterAggressionComponent.MaxAggressionTarget == null)
                return;

            aiStateMachineController.SwitchState<MoveToTargetByNavMeshAiState>();
        }

        public override void OnStateAsyncUpdate()
        {
        }
        
        public override void OnStateBegin()
        {
            characterLiveComponent.OnCharacterGetDamageBy += OnCharacterGetDamageBy;
        }

        public override void OnStateEnd()
        {
            characterLiveComponent.OnCharacterGetDamageBy -= OnCharacterGetDamageBy;
        }

        private void OnCharacterGetDamageBy(ICharacterEntity aggressor)
        {
            if (characterLiveComponent.IsAlive)
                characterAggressionComponent.AddAggressionScore(aggressor, 10);
        }
    }
}