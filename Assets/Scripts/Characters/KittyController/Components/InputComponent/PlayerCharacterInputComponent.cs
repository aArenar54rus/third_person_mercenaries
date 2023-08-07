using UnityEngine;


namespace Arenar.Character
{
    public class PlayerCharacterInputComponent : ICharacterInputComponent
    {
        private PlayerInput playerInput;
        
        
        public PlayerInput PlayerInputs
        {
            get
            {
                playerInput ??= new PlayerInput();
                
                return playerInput;
            }
        }
        
        public Vector2 MoveAction =>
            PlayerInputs.Player.Move.ReadValue<Vector2>();
        
        public Vector2 LookAction =>
            PlayerInputs.Player.Look.ReadValue<Vector2>();

        public bool JumpAction =>
            PlayerInputs.Player.Jump.IsPressed();

        public bool SprintAction =>
            PlayerInputs.Player.Sprint.IsPressed();

        public bool InteractAction =>
            PlayerInputs.Player.Interact.WasPressedThisFrame();

        public bool AimAction =>
            PlayerInputs.Player.Aim.IsPressed();

        public bool AttackAction =>
            PlayerInputs.Player.Attack.WasPressedThisFrame();


        public void SetControlStatus(bool status)
        {
            if (status)
                PlayerInputs.Player.Enable();
            else
                PlayerInputs.Player.Disable();
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