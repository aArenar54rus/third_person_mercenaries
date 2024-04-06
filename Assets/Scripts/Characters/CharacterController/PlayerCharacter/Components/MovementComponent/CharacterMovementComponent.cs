using UnityEngine;
using Zenject;


namespace Arenar.Character
{
    public class CharacterMovementComponent : ICharacterMovementComponent, ITickable
    {
        private const float THRESHOLD = 0.01f;
        
        
        private ICharacterEntity character;
        private CharacterPhysicsDataStorage characterPhysicsDataStorage;
        private PlayerCharacterParametersData _playerCharacterParametersData;
        private TickableManager tickableManager;
        private PlayerInput playerInput;
        private Camera mainCamera;
        
        private ICharacterLiveComponent characterLiveComponent;
        private ICharacterRayCastComponent characterRayCastComponent;
        private ICharacterInputComponent characterInputComponent;
        private CharacterAnimationComponent characterAnimationComponent;

        // player
        private float speed;
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
        
        // rotate
        private bool IsRotateOnMove =>
            !characterInputComponent.AimAction;
        

        private Transform characterTransform =>
            character.CharacterTransform;

        private PlayerInput PlayerInputs
        {
            get
            {
                playerInput ??= new PlayerInput();
                playerInput.Player.Enable();
                return playerInput;
            }
        }

        private Vector3 InputDirection =>
            new Vector3(characterInputComponent.MoveAction.x, 0.0f, characterInputComponent.MoveAction.y).normalized;



        [Inject]
        public void Construct(ICharacterEntity character,
                              Camera camera,
                              TickableManager tickableManager,
                              ICharacterDataStorage<CharacterPhysicsDataStorage> characterPhysicsDataStorage,
                              PlayerCharacterParametersData playerCharacterParametersData)
        {
            mainCamera = camera;
            this.character = character;
            this._playerCharacterParametersData = playerCharacterParametersData;
            this.tickableManager = tickableManager;
            this.characterPhysicsDataStorage = characterPhysicsDataStorage.Data;
        }

        public void Initialize()
        {
            tickableManager.Add(this);
            
            character.TryGetCharacterComponent<ICharacterLiveComponent>(out characterLiveComponent);
            character.TryGetCharacterComponent<ICharacterRayCastComponent>(out characterRayCastComponent);
            character.TryGetCharacterComponent<ICharacterInputComponent>(out characterInputComponent);
            
            if (character.TryGetCharacterComponent<ICharacterAnimationComponent>(out ICharacterAnimationComponent animComponent))
            {
                if (animComponent is CharacterAnimationComponent characterAnimationComponent)
                    this.characterAnimationComponent = characterAnimationComponent;
            }
        }

        public void DeInitialize() =>
            tickableManager.Remove(this);

        public void OnStart() { }

        public void Tick()
        {
            if (!characterLiveComponent.IsAlive)
                return;
            
            JumpAndGravity();
            Move(default);
            Rotation(default);
            CameraRotation();
        }

        public void Move(Vector3 direction)
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = characterInputComponent.SprintAction
                ? _playerCharacterParametersData.SprintSpeed
                : _playerCharacterParametersData.MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (characterInputComponent.MoveAction == Vector2.zero)
                targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(characterPhysicsDataStorage.CharacterController.velocity.x, 0.0f,
                characterPhysicsDataStorage.CharacterController.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = characterInputComponent.MoveAction.magnitude;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset
                || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * _playerCharacterParametersData.SpeedChangeRate);

                // round speed to 3 decimal places
                speed = Mathf.Round(speed * 1000f) / 1000f;
            }
            else
            {
                speed = targetSpeed;
            }

            animationBlend = Mathf.Lerp(animationBlend, targetSpeed,
                Time.deltaTime * _playerCharacterParametersData.SpeedChangeRate);

            if (animationBlend < 0.01f)
                animationBlend = 0f;

            Vector3 targetDirection = Vector3.zero;
            if (IsRotateOnMove)
                targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;
            else
            {
                targetDirection = Quaternion.Euler(0.0f,
                                      Mathf.Atan2(InputDirection.x, InputDirection.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y,
                                      0.0f) * Vector3.forward;
            }

            // move the player
            characterPhysicsDataStorage.CharacterController.Move(targetDirection.normalized * (speed * Time.deltaTime) +
                             new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
            
            float runningAnimMultiplier = characterInputComponent.SprintAction ? 1.0f : 0.5f;
            float runningAnimMultiplierX = runningAnimMultiplier * characterInputComponent.MoveAction.x;
            float runningAnimMultiplierY = runningAnimMultiplier * characterInputComponent.MoveAction.y;

            animationBlendX = Mathf.Lerp(animationBlendX, runningAnimMultiplierX,
                Time.deltaTime * _playerCharacterParametersData.SpeedChangeRate);
            animationBlendY = Mathf.Lerp(animationBlendY, runningAnimMultiplierY,
                Time.deltaTime * _playerCharacterParametersData.SpeedChangeRate);

            float animationSpeedBlend = 0;
            if (animationBlend > 0.05f)
                animationSpeedBlend = currentHorizontalSpeed / _playerCharacterParametersData.MoveSpeed * runningAnimMultiplier;

            characterAnimationComponent.SetAnimationValue(Character.CharacterAnimationComponent.AnimationValue.Speed, animationSpeedBlend);
            characterAnimationComponent.SetAnimationValue(Character.CharacterAnimationComponent.AnimationValue.MotionSpeedX, animationBlendX);
            characterAnimationComponent.SetAnimationValue(Character.CharacterAnimationComponent.AnimationValue.MotionSpeedY, animationBlendY);
        }

        public void Rotation(Vector3 direction)
        {
            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving

            if (IsRotateOnMove)
            {
                if (characterInputComponent.MoveAction == Vector2.zero)
                    return;
                
                targetRotation = Mathf.Atan2(InputDirection.x, InputDirection.z) * Mathf.Rad2Deg +
                                 mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(characterTransform.eulerAngles.y, targetRotation,
                    ref rotationVelocity,
                    _playerCharacterParametersData.RotationSmoothTime);

                // rotate to face input direction relative to camera position
                characterTransform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
            else
            {
                targetRotation = mainCamera.transform.eulerAngles.y;

                float rotation = Mathf.SmoothDampAngle(characterTransform.eulerAngles.y, targetRotation,
                    ref rotationVelocity,
                    _playerCharacterParametersData.RotationSmoothTime);
                
                characterTransform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
        }

        private void JumpAndGravity()
        {
            if (characterRayCastComponent.IsGrounded)
            {
                // reset the fall timeout timer
                fallTimeoutDelta = _playerCharacterParametersData.FallTimeout;

                // update animator if using character
                characterAnimationComponent.SetAnimationValue(CharacterAnimationComponent.AnimationValue.Grounded, 1); 
                characterAnimationComponent.SetAnimationValue(CharacterAnimationComponent.AnimationValue.Jump, 0);
                characterAnimationComponent.SetAnimationValue(CharacterAnimationComponent.AnimationValue.FreeFall, 0);

                // stop our velocity dropping infinitely when grounded
                if (verticalVelocity < 0.0f)
                    verticalVelocity = -2f;
                
                // Jump
                if (characterInputComponent.JumpAction && jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    verticalVelocity = Mathf.Sqrt(_playerCharacterParametersData.JumpHeight * -2f * _playerCharacterParametersData.Gravity);
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
                jumpTimeoutDelta = _playerCharacterParametersData.JumpTimeout;

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
                verticalVelocity += _playerCharacterParametersData.Gravity * Time.deltaTime;
            }
        }
        
        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (characterInputComponent.LookAction.sqrMagnitude >= THRESHOLD && !_playerCharacterParametersData.LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                //float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                float sensitivity = GetSensitivity();

                cinemachineTargetYaw += characterInputComponent.LookAction.x * sensitivity;// * deltaTimeMultiplier;
                cinemachineTargetPitch += characterInputComponent.LookAction.y * sensitivity;// * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
            cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, _playerCharacterParametersData.BottomClamp, _playerCharacterParametersData.TopClamp);

            // Cinemachine will follow this target
            characterPhysicsDataStorage.CameraTransform.transform.rotation = Quaternion.Euler(cinemachineTargetPitch + _playerCharacterParametersData.CameraAngleOverride,
                cinemachineTargetYaw, 0.0f);
        }
        
        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private float GetSensitivity()
        {
            if (characterInputComponent.AimAction)
                return _playerCharacterParametersData.AimCameraSensitivity;
            else return _playerCharacterParametersData.DefaultCameraSensitivity;
        }
    }
}