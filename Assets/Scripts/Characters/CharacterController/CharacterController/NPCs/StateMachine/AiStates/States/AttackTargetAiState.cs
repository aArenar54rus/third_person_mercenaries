using UnityEngine;


namespace Arenar.Character
{
    public class AttackTargetAiState : AIState
    {
        private ICharacterAggressionComponent characterAggressionComponent;
        private ICharacterAttackComponent characterAttackComponent;
        private ICharacterMovementComponent characterMovementComponent;
        
        private readonly float distanceToTarget = 1;
        
        
        public override void Initialize(ICharacterEntity character)
        {
            base.Initialize(character);
            
            character.TryGetCharacterComponent(out characterAttackComponent);
            character.TryGetCharacterComponent(out characterAggressionComponent);
            character.TryGetCharacterComponent(out characterMovementComponent);
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
                Vector3 direction = characterAggressionComponent.MaxAggressionTarget.CharacterTransform.position
                    - character.CharacterTransform.position;
                direction.y = 0;
                direction.Normalize();
            
                characterMovementComponent.Move(Vector3.zero, false);
                characterMovementComponent.Rotation(new Vector2(direction.x, direction.z));
                
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