using UnityEngine;
using UnityEngine.InputSystem;


namespace Arenar.Services.PlayerInputService
{
    public class PlayerInputService : IPlayerInputService
    {
        private PlayerInput _playerInput;


        public IInputActionCollection InputActionCollection => _playerInput;


        public PlayerInputService()
        {
            _playerInput ??= new PlayerInput();
        }
        

        public void SetInputControlType(InputActionMapType type, bool status)
        {
            switch (type)
            {
                case InputActionMapType.UI:
                    if (status)
                    {
                        _playerInput.UI.Enable();
                        Cursor.lockState = CursorLockMode.None;
                    }
                    else
                    {
                        _playerInput.UI.Disable();
                    }

                    break;
                
                case InputActionMapType.Gameplay:
                    if (status)
                    {
                        _playerInput.Player.Enable();
                        Cursor.lockState = CursorLockMode.Locked;
                    }
                    else
                    {
                        _playerInput.Player.Disable();
                    }

                    break;
                
                case InputActionMapType.None:
                    break;
                
                default:
                    Debug.LogError($"Unknown type {type}");
                    break;
            }
        }
    }
}