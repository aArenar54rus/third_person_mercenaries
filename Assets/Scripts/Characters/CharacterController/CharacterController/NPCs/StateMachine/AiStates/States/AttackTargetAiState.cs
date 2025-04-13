namespace Arenar.Character
{
    public class AttackTargetAiState : AIState
    {
        private ICharacterAggressionComponent characterAggressionComponent;
        private ICharacterAttackComponent characterAttackComponent;
        
        private readonly float distanceToTarget = 1;
        
        
        public override void Initialize(ICharacterEntity character)
        {
            base.Initialize(character);
            
            character.TryGetCharacterComponent(out characterAttackComponent);
            character.TryGetCharacterComponent(out characterAggressionComponent);
        }
        
        public override void DeInitialize()
        {
        }
        
        public override void OnStateBegin()
        {
        }
        
        public override void OnStateSyncUpdate()
        {
            if (characterAggressionComponent.MaxAggressionTarget != null)
            {
                if (characterAttackComponent.HasProcess)
                    return;
                
                float distanceSquared = (characterAggressionComponent.MaxAggressionTarget.CharacterTransform.position
                    - character.CharacterTransform.position).sqrMagnitude;

                bool targetOnDistance = distanceSquared <= distanceToTarget * distanceToTarget;
                
                if (targetOnDistance)
                    characterAttackComponent.PlayAction();
                else
                    aiStateMachineController.SwitchState<MoveToTargetByNavMeshAiState>();
            }
            else
            {
                aiStateMachineController.SwitchState<SearchTargetAiState>();
            }
        }
        
        public override void OnStateAsyncUpdate()
        {
        }
        
        public override void OnStateEnd()
        {
        }
    }
}