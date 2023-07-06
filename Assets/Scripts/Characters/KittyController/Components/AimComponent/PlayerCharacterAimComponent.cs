using Arenar.CameraService;
using UnityEngine;
using Zenject;


namespace Arenar.Character
{
    public class PlayerCharacterAimComponent : IAimComponent, ITickable
    {
        private ICharacterEntity character;
        private TickableManager tickableManager;
        private ICameraService cameraService;
        
        
        private ICharacterInputComponent _inputComponent;


        public bool IsAim { get; private set; } = false;


        [Inject]
        public void Construct(ICharacterEntity character,
            TickableManager tickableManager,
            ICameraService cameraService)
        {
            this.character = character;
            this.tickableManager = tickableManager;
            this.cameraService = cameraService;
        }
        
        
        public void Initialize()
        {
            _inputComponent = character.TryGetCharacterComponent<ICharacterInputComponent>(out bool isSuccess);
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

            if (IsAim)
            {
                cameraService.SetCinemachineVirtualCamera(CinemachineCameraType.AimTPS);
            }
            else
            {
                cameraService.SetCinemachineVirtualCamera(CinemachineCameraType.DefaultTPS);
            }
        }
    }
}