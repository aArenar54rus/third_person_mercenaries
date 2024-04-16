using Arenar.CameraService;
using UnityEngine;
using Zenject;


namespace Arenar.Character
{
    public class PlayerCharacterAimComponent : ICharacterAimComponent, ITickable
    {
        private ICharacterEntity character;
        private TickableManager tickableManager;
        private ICameraService cameraService;


        private ICharacterInputComponent _inputComponent;

        private float _aimAnimationProcess = 0.0f;


        public bool IsAim { get; private set; } = false;
        
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
            
            tickableManager.Add(this);
        }

        public void DeInitialize()
        {
            tickableManager.Remove(this);
        }

        public void OnStart()
        {
            IsAim = false;
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
                _aimAnimationProcess = Mathf.Clamp01(_aimAnimationProcess + Time.deltaTime);
            }
            else
            {
                _aimAnimationProcess = 0;
            }

            CharacterAnimationComponent.SetAnimationValue(CharacterAnimationComponent.AnimationValue.Aim, _aimAnimationProcess);
        }
    }
}