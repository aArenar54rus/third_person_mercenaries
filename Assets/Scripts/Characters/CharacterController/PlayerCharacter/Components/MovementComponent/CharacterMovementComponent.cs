using UnityEngine;
using Zenject;


namespace Arenar.Character
{
    public class CharacterMovementComponent : ICharacterMovementComponent
    {
        private ICharacterEntity characterOwner;
        private CharacterPhysicsDataStorage characterPhysicsDataStorage;
        private PlayerCharacterParametersData playerCharacterParametersData;
        private PlayerInput playerInput;
        private Camera mainCamera;
        
        private ICharacterLiveComponent characterLiveComponent;
        private ICharacterRayCastComponent characterRayCastComponent;
        private CharacterAnimationComponent characterAnimationComponent;

        // player
        private float animationBlend;
        private float targetRotation = 0.0f;
        private float rotationVelocity;
        private float verticalVelocity;
        private float terminalVelocity = 53.0f;
        
        // animation
        private float animationBlendX = 0.0f;
        private float animationBlendY = 0.0f;

        // timeout deltatime
        private float jumpTimeoutDelta;
        private float fallTimeoutDelta;
        
        // camera
        private float cinemachineTargetYaw;
        private float cinemachineTargetPitch;

        private ICharacterAimComponent _aimComponent;
        
        
        public MovementContainer MovementContainer { get; set; }
        
        // rotate
        private bool IsRotateOnMove =>
            !AimComponent.IsAim;

        private Transform CharacterTransform =>
            characterOwner.CharacterTransform;

        private ICharacterAimComponent AimComponent
        {
            get
            {
                if (_aimComponent == null)
                    characterOwner.TryGetCharacterComponent<ICharacterAimComponent>(out _aimComponent);
                return _aimComponent;
            }
        }

        
        [Inject]
        public void Construct(ICharacterEntity character,
                              Camera camera,
                              ICharacterDataStorage<CharacterPhysicsDataStorage> characterPhysicsDataStorage,
                              PlayerCharacterParametersData playerCharacterParametersData)
        {
            mainCamera = camera;
            characterOwner = character;
            this.playerCharacterParametersData = playerCharacterParametersData;
            this.characterPhysicsDataStorage = characterPhysicsDataStorage.Data;
        }

        public void Initialize()
        {
            characterOwner.TryGetCharacterComponent<ICharacterLiveComponent>(out characterLiveComponent);
            characterOwner.TryGetCharacterComponent<ICharacterRayCastComponent>(out characterRayCastComponent);
            
            if (characterOwner.TryGetCharacterComponent<ICharacterAnimationComponent>(out ICharacterAnimationComponent animComponent))
            {
                if (animComponent is CharacterAnimationComponent characterAnimationComponent)
                    this.characterAnimationComponent = characterAnimationComponent;
            }

            MovementContainer = new MovementContainer();
            MovementContainer.MovementSpeed = playerCharacterParametersData.MoveSpeed;
            MovementContainer.SprintSpeed = playerCharacterParametersData.SprintSpeed;
        }

        public void DeInitialize() { }

        public void OnActivate() {}
        
        public void OnDeactivate() {}
        
        public void Move(Vector2 direction, bool isSprint)
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = isSprint
                ? MovementContainer.SprintSpeed
                : MovementContainer.MovementSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (direction == Vector2.zero)
                targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(characterPhysicsDataStorage.CharacterController.velocity.x, 0.0f,
                characterPhysicsDataStorage.CharacterController.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = direction.magnitude;
            float speed;
            
            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset
                || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * playerCharacterParametersData.SpeedChangeRate);

                // round speed to 3 decimal places
                speed = Mathf.Round(speed * 1000f) / 1000f;
            }
            else
            {
                speed = targetSpeed;
            }

            animationBlend = Mathf.Lerp(animationBlend, targetSpeed,
                Time.deltaTime * playerCharacterParametersData.SpeedChangeRate);

            if (animationBlend < 0.01f)
                animationBlend = 0f;

            Vector3 targetDirection = Vector3.zero;
            if (IsRotateOnMove)
                targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;
            else
            {
                targetDirection = Quaternion.Euler(0.0f,
                    Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y,
                    0.0f) * Vector3.forward;
            }

            // move the player
            characterPhysicsDataStorage.CharacterController.Move(targetDirection.normalized * (speed * Time.deltaTime) +
                new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
            
            float runningAnimMultiplier = isSprint ? 1.0f : 0.5f;
            float runningAnimMultiplierX = runningAnimMultiplier * direction.x;
            float runningAnimMultiplierY = runningAnimMultiplier * direction.y;

            animationBlendX = Mathf.Lerp(animationBlendX, runningAnimMultiplierX,
                Time.deltaTime * playerCharacterParametersData.SpeedChangeRate);
            animationBlendY = Mathf.Lerp(animationBlendY, runningAnimMultiplierY,
                Time.deltaTime * playerCharacterParametersData.SpeedChangeRate);

            float animationSpeedBlend = 0;
            if (animationBlend > 0.05f)
                animationSpeedBlend = currentHorizontalSpeed / playerCharacterParametersData.MoveSpeed * runningAnimMultiplier;

            characterAnimationComponent.SetAnimationValue(Character.CharacterAnimationComponent.AnimationValue.Speed, animationSpeedBlend);
            characterAnimationComponent.SetAnimationValue(Character.CharacterAnimationComponent.AnimationValue.MotionSpeedX, animationBlendX);
            characterAnimationComponent.SetAnimationValue(Character.CharacterAnimationComponent.AnimationValue.MotionSpeedY, animationBlendY);
        }

        public void Rotation(Vector2 direction)
        {
            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving

            if (IsRotateOnMove)
            {
                if (direction == Vector2.zero)
                    return;
                
                targetRotation = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg +
                    mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(CharacterTransform.eulerAngles.y, targetRotation,
                    ref rotationVelocity,
                    playerCharacterParametersData.RotationSmoothTime);

                // rotate to face input direction relative to camera position
                CharacterTransform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
            else
            {
                targetRotation = mainCamera.transform.eulerAngles.y;

                float rotation = Mathf.SmoothDampAngle(CharacterTransform.eulerAngles.y, targetRotation,
                    ref rotationVelocity,
                    playerCharacterParametersData.RotationSmoothTime);
                
                CharacterTransform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
        }

        public void JumpAndGravity(bool jumpAction)
        {
            if (characterRayCastComponent.IsGrounded)
            {
                // reset the fall timeout timer
                fallTimeoutDelta = playerCharacterParametersData.FallTimeout;

                // update animator if using character
                characterAnimationComponent.SetAnimationValue(CharacterAnimationComponent.AnimationValue.Grounded, 1); 
                characterAnimationComponent.SetAnimationValue(CharacterAnimationComponent.AnimationValue.Jump, 0);
                characterAnimationComponent.SetAnimationValue(CharacterAnimationComponent.AnimationValue.FreeFall, 0);

                // stop our velocity dropping infinitely when grounded
                if (verticalVelocity < 0.0f)
                    verticalVelocity = -2f;
                
                // Jump
                if (playerCharacterParametersData.CanJump && jumpAction && jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    verticalVelocity = Mathf.Sqrt(playerCharacterParametersData.JumpHeight * -2f * playerCharacterParametersData.Gravity);
                    characterAnimationComponent.SetAnimationValue(Character.CharacterAnimationComponent.AnimationValue.Jump, 1);
                }

                // jump timeout
                if (jumpTimeoutDelta >= 0.0f)
                    jumpTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                characterAnimationComponent.SetAnimationValue(CharacterAnimationComponent.AnimationValue.Grounded, 0); 
                // reset the jump timeout timer
                jumpTimeoutDelta = playerCharacterParametersData.JumpTimeout;

                // fall timeout
                if (fallTimeoutDelta >= 0.0f)
                {
                    fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    characterAnimationComponent.SetAnimationValue(Character.CharacterAnimationComponent.AnimationValue.FreeFall, 1);
                }
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (verticalVelocity < terminalVelocity)
            {
                verticalVelocity += playerCharacterParametersData.Gravity * Time.deltaTime;
            }
        }
    }
}