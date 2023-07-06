using StarterAssets;
using UnityEngine;
using Zenject;


namespace Arenar.Character
{
    public class CharacterMovementComponent : ICharacterMovementComponent, ITickable
    {
        private const float THRESHOLD = 0.01f;
        
        
        private ICharacterEntity kittyCharacter;
        private CharacterPhysicsDataStorage characterPhysicsDataStorage;
        private PlayerCharacterParametersData _playerCharacterParametersData;
        private TickableManager tickableManager;
        private PlayerInput playerInput;
        private Camera mainCamera;

        // player
        private float speed;
        private float animationBlend;
        private float targetRotation = 0.0f;
        private float rotationVelocity;
        private float verticalVelocity;
        private float terminalVelocity = 53.0f;

        // timeout deltatime
        private float jumpTimeoutDelta;
        private float fallTimeoutDelta;
        
        // camera
        private float cinemachineTargetYaw;
        private float cinemachineTargetPitch;
        
        // rotate
        private bool IsRotateOnMove =>
            !CharacterInputComponent.AimAction;
        

        private Transform characterTransform =>
            kittyCharacter.CharacterTransform;

        private PlayerInput PlayerInputs
        {
            get
            {
                playerInput ??= new PlayerInput();
                playerInput.Player.Enable();
                return playerInput;
            }
        }

        private ICharacterLiveComponent CharacterLiveComponent { get; set; }
            
        private ICharacterRayCastComponent CharacterRayCastComponent { get; set; }

        private ICharacterInputComponent CharacterInputComponent { get; set; }
        private CharacterAnimationComponent CharacterAnimationComponent { get; set; }
        
        private Vector3 InputDirection =>
            new Vector3(CharacterInputComponent.MoveAction.x, 0.0f, CharacterInputComponent.MoveAction.y).normalized;



        [Inject]
        public void Construct(ICharacterEntity kittyCharacter,
            Camera camera,
            TickableManager tickableManager,
            ICharacterDataStorage<CharacterPhysicsDataStorage> characterPhysicsDataStorage,
            PlayerCharacterParametersData playerCharacterParametersData)
        {
            mainCamera = camera;
            this.kittyCharacter = kittyCharacter;
            this._playerCharacterParametersData = playerCharacterParametersData;
            this.tickableManager = tickableManager;
            this.characterPhysicsDataStorage = characterPhysicsDataStorage.Data;
        }

        public void Initialize()
        {
            tickableManager.Add(this);
            
            CharacterLiveComponent = kittyCharacter.TryGetCharacterComponent<ICharacterLiveComponent>(out bool isSuccessCharacterLiveComponent);
            CharacterRayCastComponent = kittyCharacter.TryGetCharacterComponent<ICharacterRayCastComponent>(out bool isSuccessCharacterRayCastComponent);
            CharacterInputComponent = kittyCharacter.TryGetCharacterComponent<ICharacterInputComponent>(out bool isSuccessCharacterInputComponent);
            
            var iCharacterAnimationComponent = kittyCharacter.TryGetCharacterComponent<ICharacterAnimationComponent>(out bool isSuccessCharacterAnimationComponent);
            if (isSuccessCharacterAnimationComponent)
            {
                if (iCharacterAnimationComponent is CharacterAnimationComponent characterAnimationComponent)
                    CharacterAnimationComponent = characterAnimationComponent;
            }
        }

        public void DeInitialize() =>
            tickableManager.Remove(this);

        public void OnStart() { }

        public void Tick()
        {
            if (!CharacterLiveComponent.IsAlive)
                return;
            
            JumpAndGravity();
            Move();
            Rotation();
            CameraRotation();
        }

        public void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = CharacterInputComponent.SprintAction
                ? _playerCharacterParametersData.SprintSpeed
                : _playerCharacterParametersData.MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (CharacterInputComponent.MoveAction == Vector2.zero)
                targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(characterPhysicsDataStorage.CharacterController.velocity.x, 0.0f,
                characterPhysicsDataStorage.CharacterController.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = CharacterInputComponent.MoveAction.magnitude;

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

            CharacterAnimationComponent.SetAnimationValue(Character.CharacterAnimationComponent.KittyAnimationValue.Speed, animationBlend);
            CharacterAnimationComponent.SetAnimationValue(Character.CharacterAnimationComponent.KittyAnimationValue.MotionSpeed, inputMagnitude);
        }

        public void Rotation()
        {
            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving

            if (IsRotateOnMove)
            {
                if (CharacterInputComponent.MoveAction == Vector2.zero)
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
            if (CharacterRayCastComponent.IsGroundedCheck())
            {
                // reset the fall timeout timer
                fallTimeoutDelta = _playerCharacterParametersData.FallTimeout;

                // update animator if using character
                CharacterAnimationComponent.SetAnimationValue(CharacterAnimationComponent.KittyAnimationValue.Grounded, 1); 
                CharacterAnimationComponent.SetAnimationValue(Character.CharacterAnimationComponent.KittyAnimationValue.Jump, 0);
                CharacterAnimationComponent.SetAnimationValue(Character.CharacterAnimationComponent.KittyAnimationValue.FreeFall, 0);

                // stop our velocity dropping infinitely when grounded
                if (verticalVelocity < 0.0f)
                {
                    verticalVelocity = -2f;
                }

                // Jump
                if (CharacterInputComponent.JumpAction && jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    verticalVelocity = Mathf.Sqrt(_playerCharacterParametersData.JumpHeight * -2f * _playerCharacterParametersData.Gravity);
                    CharacterAnimationComponent.SetAnimationValue(Character.CharacterAnimationComponent.KittyAnimationValue.Jump, 1);
                }

                // jump timeout
                if (jumpTimeoutDelta >= 0.0f)
                    jumpTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                CharacterAnimationComponent.SetAnimationValue(CharacterAnimationComponent.KittyAnimationValue.Grounded, 0); 
                // reset the jump timeout timer
                jumpTimeoutDelta = _playerCharacterParametersData.JumpTimeout;

                // fall timeout
                if (fallTimeoutDelta >= 0.0f)
                {
                    fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    CharacterAnimationComponent.SetAnimationValue(Character.CharacterAnimationComponent.KittyAnimationValue.FreeFall, 1);
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
            if (CharacterInputComponent.LookAction.sqrMagnitude >= THRESHOLD && !_playerCharacterParametersData.LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                //float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                float sensitivity = GetSensitivity();

                cinemachineTargetYaw += CharacterInputComponent.LookAction.x * sensitivity;// * deltaTimeMultiplier;
                cinemachineTargetPitch += CharacterInputComponent.LookAction.y * sensitivity;// * deltaTimeMultiplier;
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
            if (CharacterInputComponent.AimAction)
                return _playerCharacterParametersData.AimCameraSensitivity;
            else return _playerCharacterParametersData.DefaultCameraSensitivity;
        }
    }
}