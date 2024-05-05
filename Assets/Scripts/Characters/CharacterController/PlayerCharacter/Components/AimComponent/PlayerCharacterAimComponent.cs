using Arenar.CameraService;
using UnityEngine;
using Zenject;


namespace Arenar.Character
{
    public class PlayerCharacterAimComponent : ICharacterAimComponent, ITickable
    {
        private const float AIM_SPEED_MULTIPLIER = 3.0f;
        
        
        private ICharacterEntity character;
        private TickableManager tickableManager;
        private ICameraService cameraService;
        
        private ICharacterInputComponent _inputComponent;


        public bool IsAim { get; private set; } = false;

        public float AimProgress { get; private set; } = 0.0f;
        
        private CharacterAnimationComponent CharacterAnimationComponent { get; set; }


        [Inject]
        public void Construct(ICharacterEntity character,
                              TickableManager tickableManager,
                              ICameraService cameraService,
                              PlayerCharacterParametersData playerCharacterParametersData)
        {
            this.character = character;
            this.tickableManager = tickableManager;
            this.cameraService = cameraService;
        }
        
        
        public void Initialize()
        {
            character.TryGetCharacterComponent<ICharacterInputComponent>(out _inputComponent);
            if (character.TryGetCharacterComponent<ICharacterAnimationComponent>(out ICharacterAnimationComponent animationComponent))
            {
                if (animationComponent is CharacterAnimationComponent characterAnimationComponent)
                    CharacterAnimationComponent = characterAnimationComponent;
            }
        }

        public void DeInitialize() { }

        public void OnActivate()
        {
            tickableManager.Add(this);
            IsAim = false;
        }

        public void OnDeactivate()
        {
            tickableManager.Remove(this);
        }

        public void Tick()
        {
            AnimationAimProcess();
            
            if (_inputComponent.AimAction == IsAim)
                return;

            IsAim = _inputComponent.AimAction;

            cameraService.SetCinemachineVirtualCamera(IsAim
                ? CinemachineCameraType.AimTPS
                : CinemachineCameraType.DefaultTPS);
        }

        private void AnimationAimProcess()
        {
            if (IsAim)
            {
                AimProgress = Mathf.Clamp01(AimProgress + AIM_SPEED_MULTIPLIER * Time.deltaTime);
            }
            else
            {
                AimProgress = 0;
            }

            CharacterAnimationComponent.SetAnimationValue(CharacterAnimationComponent.AnimationValue.Aim, AimProgress);
        }
    }
}