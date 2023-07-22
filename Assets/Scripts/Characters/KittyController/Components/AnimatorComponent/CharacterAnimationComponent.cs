using UnityEngine;
using Zenject;


namespace Arenar.Character
{
    public class CharacterAnimationComponent
        : ICharacterAnimationComponent<CharacterAnimationComponent.KittyAnimation, CharacterAnimationComponent.KittyAnimationValue>
    {
        public enum KittyAnimation : byte
        {
            None = 0,
        }

        public enum KittyAnimationValue : byte
        {
            None = 0,
            Speed = 1,
            Grounded = 2,
            Jump = 3,
            FreeFall = 4,
            MotionSpeedX = 5,
            MotionSpeedY = 6,
            Aim = 7,
        }

        
        private CharacterAnimatorDataStorage characterAnimatorDataStorage;
        private ICharacterEntity characterEntity;
        
        // animation IDs
        private int animIDSpeed;
        private int animIDGrounded;
        private int animIDJump;
        private int animIDFreeFall;
        private int animIDMotionSpeedX;
        private int animIDMotionSpeedY;
        private int animIDAim;
        

        private Animator KittyAnimator =>
            characterAnimatorDataStorage.Animator;

        private ICharacterLiveComponent LiveComponent =>
            characterEntity.TryGetCharacterComponent<ICharacterLiveComponent>(out bool success);


        [Inject]
        public void Construct(ICharacterDataStorage<CharacterAnimatorDataStorage> characterAnimatorDataStorage,
                              ICharacterEntity characterEntity)
        {
            this.characterAnimatorDataStorage = characterAnimatorDataStorage.Data;
            this.characterEntity = characterEntity;
        }

        public void Initialize()
        {
            animIDSpeed = Animator.StringToHash(characterAnimatorDataStorage.SpeedAnimationName);
            animIDGrounded = Animator.StringToHash(characterAnimatorDataStorage.GroundedAnimationName);
            animIDJump = Animator.StringToHash(characterAnimatorDataStorage.JumpAnimationName);
            animIDFreeFall = Animator.StringToHash(characterAnimatorDataStorage.FreeFallAnimationName);
            animIDMotionSpeedX = Animator.StringToHash(characterAnimatorDataStorage.MotionSpeedAnimationXName);
            animIDMotionSpeedY = Animator.StringToHash(characterAnimatorDataStorage.MotionSpeedAnimationYName);
            animIDAim = Animator.StringToHash(characterAnimatorDataStorage.AimAnimationName);
        }

        public void DeInitialize()
        {
        }

        public void OnStart() { }

        public void PlayAnimation(KittyAnimation animationType)
        {
            return;
            
            switch (animationType)
            {
                default:
                    Debug.LogError($"Not found animation {animationType} for kitty!");
                    break;
            }
        }

        public void SetAnimationValue(KittyAnimationValue animationValue, float value)
        {
            switch (animationValue)
            {
                case KittyAnimationValue.Speed:
                    SetAnimationFloat(animIDSpeed, value);
                    break;
                
                case KittyAnimationValue.MotionSpeedX:
                    SetAnimationFloat(animIDMotionSpeedX, value);
                    break;
                
                case KittyAnimationValue.MotionSpeedY:
                    SetAnimationFloat(animIDMotionSpeedY, value);
                    break;
                
                case KittyAnimationValue.Jump:
                    SetAnimationBool(animIDJump, value > 0);
                    break;
                
                case KittyAnimationValue.FreeFall:
                    SetAnimationBool(animIDFreeFall, value > 0);
                    break;
                
                case KittyAnimationValue.Grounded:
                    SetAnimationBool(animIDGrounded, value > 0);
                    break;
                
                case KittyAnimationValue.Aim:
                    SetAnimationBool(animIDAim, value > 0);
                    break;
                
                default:
                    Debug.LogError($"Unknown type {animationValue} for character animation.");
                    break;
            }
        }

        private void PlayAnimation(string animationKey) =>
            KittyAnimator.Play(animationKey);

        private void SetAnimationTrigger(string triggerName) =>
            KittyAnimator.SetTrigger(triggerName);

        private void SetAnimationBool(int animationIndex, bool value) =>
            KittyAnimator.SetBool(animationIndex, value);
        
        private void SetAnimationFloat(int animationIndex, float value) =>
            KittyAnimator.SetFloat(animationIndex, value);
    }
}