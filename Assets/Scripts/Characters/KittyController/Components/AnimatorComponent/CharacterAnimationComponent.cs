using UnityEngine;
using Zenject;


namespace Arenar.Character
{
    public class CharacterAnimationComponent
        : ICharacterAnimationComponent<CharacterAnimationComponent.KittyAnimation, CharacterAnimationComponent.KittyAnimationValue>, ITickable
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
            HandPistol = 8,
        }


        private CharacterAnimatorDataStorage characterAnimatorDataStorage;
        private CharacterAimAnimationDataStorage characterAimAnimationDataStorage;
        private ICharacterEntity characterEntity;
        private TickableManager tickableManager;

        private ICharacterLiveComponent liveComponent;
        private ICharacterRayCastComponent rayCastComponent;
        private ICharacterAimComponent characterAimComponent;
        
        // animation IDs
        private int animIDSpeed;
        private int animIDGrounded;
        private int animIDJump;
        private int animIDFreeFall;
        private int animIDMotionSpeedX;
        private int animIDMotionSpeedY;
        private int animIDAim;
        private int animIDHandPistol;
        

        private Animator KittyAnimator =>
            characterAnimatorDataStorage.Animator;
        
        private bool IsFindObject =>
            (CharacterRayCastComponent.InteractableElementsOnCross != null);

        private ICharacterLiveComponent LiveComponent => liveComponent;

        private ICharacterRayCastComponent CharacterRayCastComponent => rayCastComponent;

        private ICharacterAimComponent CharacterAimComponent => characterAimComponent;


        [Inject]
        public void Construct(ICharacterDataStorage<CharacterAnimatorDataStorage> characterAnimatorDataStorage,
                              ICharacterDataStorage<CharacterAimAnimationDataStorage> characterAimDataStorage,
                              ICharacterEntity characterEntity,
                              TickableManager tickableManager)
        {
            this.characterAnimatorDataStorage = characterAnimatorDataStorage.Data;
            this.characterAimAnimationDataStorage = characterAimDataStorage.Data;
            this.characterEntity = characterEntity;
            this.tickableManager = tickableManager;
        }

        public void Initialize()
        {
            characterEntity.TryGetCharacterComponent<ICharacterLiveComponent>(out liveComponent);
            characterEntity.TryGetCharacterComponent<ICharacterRayCastComponent>(out rayCastComponent);
            characterEntity.TryGetCharacterComponent<ICharacterAimComponent>(out characterAimComponent);

            InitIndexIDs();
            
            tickableManager.Add(this);
        }

        public void DeInitialize()
        {
            tickableManager.Remove(this);
        }

        public void OnStart() {}

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
                
                case KittyAnimationValue.HandPistol:
                    SetAnimationBool(animIDHandPistol, value > 0);
                    break;
                
                default:
                    Debug.LogError($"Unknown type {animationValue} for character animation.");
                    break;
            }
        }
        
        public void Tick()
        {
            SetHeadAnimationRotation();
            SetBodyAnimationRotation();
        }

        private void PlayAnimation(string animationKey) =>
            KittyAnimator.Play(animationKey);

        private void SetAnimationTrigger(string triggerName) =>
            KittyAnimator.SetTrigger(triggerName);

        private void SetAnimationBool(int animationIndex, bool value) =>
            KittyAnimator.SetBool(animationIndex, value);
        
        private void SetAnimationFloat(int animationIndex, float value) =>
            KittyAnimator.SetFloat(animationIndex, value);

        private void SetHeadAnimationRotation()
        {
            if (CharacterAimComponent.IsAim)
            {
                characterAimAnimationDataStorage.HeadRig.weight = Mathf.Clamp01(
                    characterAimAnimationDataStorage.HeadRig.weight - Time.deltaTime);
                return;
            }

            if (IsFindObject)
            {
                characterAimAnimationDataStorage.HeadAimPointObject.position
                    = CharacterRayCastComponent.InteractableElementsOnCross.transform.position;
            }

            characterAimAnimationDataStorage.HeadRig.weight = Mathf.Clamp01(
                characterAimAnimationDataStorage.HeadRig.weight
                + Time.deltaTime * (IsFindObject ? 1 : -1));
        }
        
        private void SetBodyAnimationRotation()
        {
            bool isAim = CharacterAimComponent.IsAim;
            characterAimAnimationDataStorage.BodyRig.weight = isAim ? 1 : 0;

            if (isAim)
            {
                if (CharacterRayCastComponent.RaycastPoint != null)
                {
                    characterAimAnimationDataStorage.BodyAimPointObject.position
                        = CharacterRayCastComponent.RaycastPoint;
                }
                
                // characterAimAnimationDataStorage.BodyAimPointObject.position = raycastPoint;
            }
        }

        private void InitIndexIDs()
        {
            animIDSpeed = Animator.StringToHash(characterAnimatorDataStorage.SpeedAnimationName);
            animIDGrounded = Animator.StringToHash(characterAnimatorDataStorage.GroundedAnimationName);
            animIDJump = Animator.StringToHash(characterAnimatorDataStorage.JumpAnimationName);
            animIDFreeFall = Animator.StringToHash(characterAnimatorDataStorage.FreeFallAnimationName);
            animIDMotionSpeedX = Animator.StringToHash(characterAnimatorDataStorage.MotionSpeedAnimationXName);
            animIDMotionSpeedY = Animator.StringToHash(characterAnimatorDataStorage.MotionSpeedAnimationYName);
            animIDAim = Animator.StringToHash(characterAnimatorDataStorage.AimAnimationName);
            animIDHandPistol = Animator.StringToHash(characterAnimatorDataStorage.HandPistolAnimationNam);
        }
    }
}