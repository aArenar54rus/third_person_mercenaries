using Arenar.Services.PlayerInputService;
using UnityEngine;
using Zenject;


namespace Arenar.Character
{
    public class PlayerCharacterInputComponent : ICharacterInputComponent
    {
        private IPlayerInputService _playerInputService;


        private PlayerInput PlayerInput =>
            (PlayerInput)_playerInputService.InputActionCollection;

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


        [Inject]
        public void Construct(IPlayerInputService playerInputService)
        {
            _playerInputService = playerInputService;
        }

        public void SetControlStatus(bool status)
        {
            _playerInputService.SetInputControlType(InputActionMapType.Gameplay, status);
        }

        public void Initialize()
        {
            SetControlStatus(true);
        }

        public void DeInitialize() { }

        public void OnStart()
        {
            SetControlStatus(true);
        }
    }
}