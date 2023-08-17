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
            _inputComponent = character.TryGetCharacterComponent<ICharacterInputComponent>(out bool isSuccess);
            
            var iCharacterAnimationComponent = character.TryGetCharacterComponent<ICharacterAnimationComponent>(out bool isSuccessCharacterAnimationComponent);
            if (isSuccessCharacterAnimationComponent)
            {
                if (iCharacterAnimationComponent is CharacterAnimationComponent characterAnimationComponent)
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
            if (_inputComponent.AimAction == IsAim)
                return;

            IsAim = _inputComponent.AimAction;

            cameraService.SetCinemachineVirtualCamera(IsAim
                ? CinemachineCameraType.AimTPS
                : CinemachineCameraType.DefaultTPS);

            CharacterAnimationComponent.SetAnimationValue(CharacterAnimationComponent.KittyAnimationValue.Aim, IsAim ? 1 : 0);
        }
    }
}