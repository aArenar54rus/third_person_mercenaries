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

        private bool isAim = false;
        private bool isAimContinue = false;


        public bool IsAim
        {
            get => isAim || isAimContinue;
            set => isAim = value;
        }

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
        }
        
        
        public void Initialize()
        {
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