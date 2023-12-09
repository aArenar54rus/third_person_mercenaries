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

            Cursor.lockState = CursorLockMode.Locked;
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
            if (_inputComponent.AimAction == IsAim)
                return;

            IsAim = _inputComponent.AimAction;

            cameraService.SetCinemachineVirtualCamera(IsAim
                ? CinemachineCameraType.AimTPS
                : CinemachineCameraType.DefaultTPS);

            CharacterAnimationComponent.SetAnimationValue(CharacterAnimationComponent.AnimationValue.Aim, IsAim ? 1 : 0);
        }
    }
}