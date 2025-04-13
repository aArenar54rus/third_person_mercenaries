using Zenject;


namespace Arenar.Character
{
    public class MoveToTargetByNavMeshAiState : AIState
    {
        private ICharacterAggressionComponent characterAggressionComponent;
        private ICharacterMovementComponent characterMovementComponent;
        private ICharacterLiveComponent characterLiveComponent;
        
        private readonly float distanceToTarget = 1;
        
        
        [Inject]
        public void Construct()
        {
            
        }
        
        public override void Initialize(ICharacterEntity character)
        {
            base.Initialize(character);
            
            character.TryGetCharacterComponent(out characterLiveComponent);
            character.TryGetCharacterComponent(out characterMovementComponent);
            character.TryGetCharacterComponent(out characterAggressionComponent);
        }
        
        public override void DeInitialize()
        {
        }

        public override void OnStateSyncUpdate()
        {
            if (characterAggressionComponent.MaxAggressionTarget == null)
            {
                aiStateMachineController.SwitchState<SearchTargetAiState>();
                return;
            }
            
            characterMovementComponent.Move(characterAggressionComponent.MaxAggressionTarget.CharacterTransform.position, false);
            float distanceSquared = (characterAggressionComponent.MaxAggressionTarget.CharacterTransform.position
                - character.CharacterTransform.position).sqrMagnitude;

            bool targetOnDistance = distanceSquared <= distanceToTarget * distanceToTarget;

            if (targetOnDistance)
            {
                aiStateMachineController.SwitchState<AttackTargetAiState>();
            }
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