using System;
using UnityEngine.InputSystem;


namespace Arenar.Services.UI
{
    public class InventoryCanvasWindow : CanvasWindow
    {
        private PlayerInput playerInput;


        private bool IsClosed =>
            !gameObject.activeSelf;

        public PlayerInput PlayerInputs
        {
            get
            {
                playerInput ??= new PlayerInput();
                playerInput.Player.Enable();
                return playerInput;
            }
        }
        

        public override void Initialize()
        {
            base.Initialize();
            PlayerInputs.Player.CharacterInformationMenu.canceled += OnInteractionMenu;
        }

        private void OnInteractionMenu(InputAction.CallbackContext callbackContext)
        {
            if (IsClosed)
            {
                Show(true);
            }
            else
            {
                Hide(true);
            }
        }

        protected override void DeInitialize()
        {
            base.DeInitialize();
            PlayerInputs.Player.CharacterInformationMenu.canceled -= OnInteractionMenu;
        }
    }
}
