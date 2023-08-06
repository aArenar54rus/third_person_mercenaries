using System;
using Arenar.Services.UI;
using UnityEngine.InputSystem;


namespace Arenar.UI
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
        

        public override void Show(bool immediately = false, Action callback = null)
        {
            base.Show(immediately, callback);

            foreach (var canvasWindowLayer in canvasWindowLayers)
                canvasWindowLayer.ShowWindowLayer(immediately);
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
