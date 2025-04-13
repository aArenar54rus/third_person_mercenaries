using Arenar.CameraService;
using UnityEngine;
using Zenject;


namespace Arenar.Character
{
    public class PlayerCharacterCameraComponent : ICharacterCameraComponent, ITickable
    {
        private const float THRESHOLD = 0.01f;
        
        
        private ICharacterEntity characterOwner;
        private ICameraService cameraService;
        private TickableManager tickableManager;
        private PlayerCharacterParametersData playerCharacterParametersData;
        private CharacterPhysicsDataStorage characterPhysicsDataStorage;
        
        private bool isAiming = false;
        
        private ICharacterLiveComponent liveComponent;
        private ICharacterAimComponent aimComponent;
        
        // camera
        private float cinemachineTargetYaw;
        private float cinemachineTargetPitch;
        
        
        [Inject]
        public void Construct(ICharacterEntity characterOwner,
                              ICameraService cameraService,
                              TickableManager tickableManager,
                              ICharacterDataStorage<CharacterPhysicsDataStorage> characterPhysicsDataStorage,
                              PlayerCharacterParametersData playerCharacterParametersData)
        {
            this.characterOwner = characterOwner;
            this.cameraService = cameraService;
            this.tickableManager = tickableManager;
            this.playerCharacterParametersData = playerCharacterParametersData;
            this.characterPhysicsDataStorage = characterPhysicsDataStorage.Data;
        }
        
        public void Initialize()
        {
            characterOwner.TryGetCharacterComponent<ICharacterAimComponent>(out aimComponent);
            characterOwner.TryGetCharacterComponent<ICharacterLiveComponent>(out liveComponent);
        }
        
        public void DeInitialize()
        {
            
        }
        
        public void OnActivate()
        {
            tickableManager.Add(this);
        }
        
        public void OnDeactivate()
        {
            tickableManager.Remove(this);
            
            UpdateAimCameraState(false);
        }
        
        public void CameraRotation(Vector2 direction)
        {
            // if there is an input and camera position is not fixed
            if (direction.sqrMagnitude >= THRESHOLD && !playerCharacterParametersData.LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                //float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                float sensitivity = GetSensitivity();

                cinemachineTargetYaw += direction.x * sensitivity;// * deltaTimeMultiplier;
                cinemachineTargetPitch += direction.y * sensitivity;// * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
            cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, playerCharacterParametersData.BottomClamp, playerCharacterParametersData.TopClamp);

            // Cinemachine will follow this target
            characterPhysicsDataStorage.CameraTransform.transform.rotation = Quaternion.Euler(cinemachineTargetPitch + playerCharacterParametersData.CameraAngleOverride,
                cinemachineTargetYaw, 0.0f);
        }

        public void Tick()
        {
            if (!liveComponent.IsAlive)
            {
                UpdateAimCameraState(false);
                return;
            }

            UpdateAimCameraState(aimComponent.IsAim);
            if (isAiming == aimComponent.IsAim)
                return;

            isAiming = aimComponent.IsAim;
                
            cameraService.SetCinemachineVirtualCamera(isAiming
                ? CinemachineCameraType.AimTPS
                : CinemachineCameraType.DefaultTPS);
        }
        
        private void UpdateAimCameraState(bool state)
        {
            if (isAiming == state)
                return;

            isAiming = state;
                
            cameraService.SetCinemachineVirtualCamera(isAiming
                ? CinemachineCameraType.AimTPS
                : CinemachineCameraType.DefaultTPS);
        }
        
        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
        
        private float GetSensitivity()
        {
            return isAiming
                ? playerCharacterParametersData.AimCameraSensitivity
                : playerCharacterParametersData.DefaultCameraSensitivity;
        }
    }
}