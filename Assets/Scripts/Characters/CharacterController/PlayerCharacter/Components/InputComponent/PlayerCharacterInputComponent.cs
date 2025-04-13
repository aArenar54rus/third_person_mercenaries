using Arenar.Services.PlayerInputService;
using UnityEngine;
using Zenject;


namespace Arenar.Character
{
    public class PlayerCharacterInputComponent : ICharacterInputComponent, ITickable, ILateTickable
    {
        private ICharacterEntity characterOwner;
        private IPlayerInputService playerInputService;
        private TickableManager tickableManager;
        
        private ICharacterLiveComponent characterLiveComponent;
        private ICharacterMovementComponent characterMovementComponent;
        private ICharacterAimComponent characterAimComponent;
        private ICharacterCameraComponent characterCameraComponent;
        private ICharacterAttackComponent characterAttackComponent;


        private PlayerInput PlayerInput =>
            (PlayerInput)playerInputService.InputActionCollection;

        public Vector2 MoveAction =>
            PlayerInput.Player.Move.ReadValue<Vector2>();
        
        public Vector2 LookAction =>
            PlayerInput.Player.Look.ReadValue<Vector2>();

        public bool JumpAction =>
            PlayerInput.Player.Jump.IsPressed();

        public bool SprintAction =>
            PlayerInput.Player.Sprint.IsPressed();

        public bool InteractAction =>
            PlayerInput.Player.Interact.WasPressedThisFrame();

        public bool AimAction =>
            PlayerInput.Player.Aim.IsPressed();

        public bool AttackAction =>
            PlayerInput.Player.Attack.WasPressedThisFrame();

        public bool ReloadAction =>
            PlayerInput.Player.Reload.WasPressedThisFrame();

        public bool AimContinueAction =>
            PlayerInput.Player.AimContinue.WasPressedThisFrame();




        [Inject]
        public void Construct(ICharacterEntity characterOwner,
                              TickableManager tickableManager,
                              IPlayerInputService playerInputService)
        {
            this.characterOwner = characterOwner;
            this.playerInputService = playerInputService;
            this.tickableManager = tickableManager;
        }

        public void SetControlStatus(bool status)
        {
            playerInputService.SetInputControlType(InputActionMapType.UI, !status);
            playerInputService.SetInputControlType(InputActionMapType.Gameplay, status);
        }
        
        public void Initialize()
        {
            characterOwner.TryGetCharacterComponent<ICharacterMovementComponent>(out characterMovementComponent);
            characterOwner.TryGetCharacterComponent<ICharacterAimComponent>(out characterAimComponent);
            characterOwner.TryGetCharacterComponent<ICharacterLiveComponent>(out characterLiveComponent);
            characterOwner.TryGetCharacterComponent<ICharacterCameraComponent>(out characterCameraComponent);
            characterOwner.TryGetCharacterComponent<ICharacterAttackComponent>(out characterAttackComponent);
            
            tickableManager.Add(this);
        }
        
        public void DeInitialize()
        {
            tickableManager.Remove(this);
        }

        public void OnActivate()
        {
            SetControlStatus(true);
        }

        public void OnDeactivate() { }

        public void Tick()
        {
            if (!characterLiveComponent.IsAlive)
            {
                characterAimComponent.IsAim = false;
                return;
            }

            characterAimComponent.IsAim = AimAction;
            characterCameraComponent.CameraRotation(LookAction);
            
            characterMovementComponent.JumpAndGravity(JumpAction);
            characterMovementComponent.Move(MoveAction, (characterAimComponent.IsAim == false) && SprintAction);
            characterMovementComponent.Rotation(MoveAction);
            
            if (!characterAttackComponent.HasProcess)
            {
                if (ReloadAction)
                    characterAttackComponent.MakeReload();

                if (AttackAction)
                    characterAttackComponent.PlayAction();
            }
        }

        public void LateTick()
        {
            if (!characterLiveComponent.IsAlive)
            {
                return;
            }
            
            
        }
    }
}