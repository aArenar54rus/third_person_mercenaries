using System;
using UnityEngine;
using Zenject;


namespace Arenar.Character
{
    public class CharacterAnimationComponent
        : ICharacterAnimationComponent<CharacterAnimationComponent.Animation, CharacterAnimationComponent.AnimationValue>, ITickable
    {
        public enum Animation : byte
        {
            None = 0,
            Shoot = 1,
        }

        public enum AnimationValue : byte
        {
            None = 0,
            Speed = 1,
            Grounded = 2,
            Jump = 3,
            FreeFall = 4,
            MotionSpeedX = 5,
            MotionSpeedY = 6,
            Aim = 7,
            PistolHands = 8,
            ShotgunHands = 9,
            RifleHands = 10,
        }

        
        public event Action<AnimationEvent> onAnimationEvent;
        

        private CharacterAnimatorDataStorage characterAnimatorDataStorage;
        private CharacterAimAnimationDataStorage characterAimAnimationDataStorage;
        private ICharacterEntity characterEntity;
        private TickableManager tickableManager;

        private ICharacterLiveComponent liveComponent;
        private ICharacterRayCastComponent rayCastComponent;
        private ICharacterAimComponent characterAimComponent;

        private PlayerCharacterParametersData playerCharacterParametersData;
        
        private float _aimAnimationProcess = 0.0f;


        private Animator Animator =>
            characterAnimatorDataStorage.Animator;
        
        private bool IsFindObject =>
            (CharacterRayCastComponent.InteractableElementsOnCross != null);

        private ICharacterRayCastComponent CharacterRayCastComponent => rayCastComponent;

        private ICharacterAimComponent CharacterAimComponent => characterAimComponent;


        [Inject]
        public void Construct(ICharacterDataStorage<CharacterAnimatorDataStorage> characterAnimatorDataStorage,
                              ICharacterDataStorage<CharacterAimAnimationDataStorage> characterAimDataStorage,
                              ICharacterEntity characterEntity,
                              TickableManager tickableManager,
                              PlayerCharacterParametersData playerCharacterParametersData)
        {
            this.characterAnimatorDataStorage = characterAnimatorDataStorage.Data;
            this.characterAimAnimationDataStorage = characterAimDataStorage.Data;
            this.characterEntity = characterEntity;
            this.tickableManager = tickableManager;
            this.playerCharacterParametersData = playerCharacterParametersData;
        }

        public void Initialize()
        {
            characterEntity.TryGetCharacterComponent<ICharacterLiveComponent>(out liveComponent);
            characterEntity.TryGetCharacterComponent<ICharacterRayCastComponent>(out rayCastComponent);
            characterEntity.TryGetCharacterComponent<ICharacterAimComponent>(out characterAimComponent);
        }

        public void DeInitialize() { }

        public void OnActivate()
        {
            tickableManager.Add(this);
        }
        
        public void OnDeactivate()
        {
            tickableManager.Remove(this);
        }

        public void PlayAnimation(Animation animationType)
        {
            switch (animationType)
            {
                case Animation.Shoot:
                    SetAnimationTrigger(characterAnimatorDataStorage.ShootAnimationName);
                    break;
                
                default:
                    Debug.LogError($"Not found animation {animationType} for kitty!");
                    break;
            }
        }

        public void SetAnimationValue(AnimationValue animationValue, float value)
        {
            switch (animationValue)
            {
                case AnimationValue.Speed:
                    SetAnimationFloat(characterAnimatorDataStorage.SpeedAnimationName, value);
                    break;
                
                case AnimationValue.MotionSpeedX:
                    SetAnimationFloat(characterAnimatorDataStorage.MotionSpeedAnimationXName, value);
                    break;
                
                case AnimationValue.MotionSpeedY:
                    SetAnimationFloat(characterAnimatorDataStorage.MotionSpeedAnimationYName, value);
                    break;
                
                case AnimationValue.Jump:
                    SetAnimationBool(characterAnimatorDataStorage.JumpAnimationName, value > 0);
                    break;
                
                case AnimationValue.FreeFall:
                    SetAnimationBool(characterAnimatorDataStorage.FreeFallAnimationName, value > 0);
                    break;
                
                case AnimationValue.Grounded:
                    SetAnimationBool(characterAnimatorDataStorage.GroundedAnimationName, value > 0);
                    break;
                
                case AnimationValue.Aim:
                    SetAnimationBool(characterAnimatorDataStorage.AimAnimationName, value > 0);
                    break;
                
                case AnimationValue.PistolHands:
                    SetAnimationBool(characterAnimatorDataStorage.HandPistolAnimationName, value > 0);
                    break;
                
                case AnimationValue.ShotgunHands:
                    SetAnimationBool(characterAnimatorDataStorage.IsHandShotgunAnimationKey, value > 0);
                    break;
                
                case AnimationValue.RifleHands:
                    SetAnimationBool(characterAnimatorDataStorage.IsHandRifleAnimationKey, value > 0);
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
            Animator.Play(animationKey);

        private void SetAnimationTrigger(string triggerName) =>
            Animator.SetTrigger(triggerName);

        private void SetAnimationBool(string boolName, bool value) =>
            Animator.SetBool(boolName, value);
        
        private void SetAnimationFloat(string floatName, float value) =>
            Animator.SetFloat(floatName, value);

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
            _aimAnimationProcess = Mathf.Clamp01(_aimAnimationProcess + (isAim ? (Time.deltaTime) : (-Time.deltaTime)) * playerCharacterParametersData.AimProcessSpeed);
            characterAimAnimationDataStorage.BodyRig.weight = _aimAnimationProcess;

            if (isAim)
            {
                if (CharacterRayCastComponent.RaycastPoint != null)
                {
                    characterAimAnimationDataStorage.BodyPistolAimPointObject.position
                        = CharacterRayCastComponent.RaycastPoint;
                }
                
                // characterAimAnimationDataStorage.BodyAimPointObject.position = raycastPoint;
            }
        }
    }
}