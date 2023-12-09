using UnityEngine;


namespace Arenar.Character
{
    public class AiIdleState : AIState
    {
        private ICharacterAnimationComponent<CharacterAnimationComponent.Animation, CharacterAnimationComponent.AnimationValue>
            characterAnimationComponent;


        public override void Initialize(ICharacterEntity character)
        {
            base.Initialize(character);
            character.TryGetCharacterComponent(out characterAnimationComponent);
        }

        public override void DeInitialize()
        {
            characterAnimationComponent = null;
        }

        public override void OnStateBegin()
        {
            characterAnimationComponent.SetAnimationValue(CharacterAnimationComponent.AnimationValue.Speed, 0);
            characterAnimationComponent.SetAnimationValue(CharacterAnimationComponent.AnimationValue.MotionSpeedX, 0);
            characterAnimationComponent.SetAnimationValue(CharacterAnimationComponent.AnimationValue.MotionSpeedY, 0);
        }

        public override void OnStateSyncUpdate()
        {
            MoveDirection = Vector3.zero;
            RotationDirection = Vector3.zero;
        }

        public override void OnStateAsyncUpdate()
        {
            return;
        }

        public override void OnStateEnd()
        {
            return;
        }
    }
}