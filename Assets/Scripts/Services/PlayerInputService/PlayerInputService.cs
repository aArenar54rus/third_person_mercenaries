using System.Collections;
using System.Collections.Generic;
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
        

        public void SetNewInputControlType(InputActionMapType type, bool status)
        {
            switch (type)
            {
                case InputActionMapType.UI:
                    if (status)
                        _playerInput.UI.Enable();
                    else
                        _playerInput.UI.Disable();
                    break;
                
                case InputActionMapType.Gameplay:
                    if (status)
                        _playerInput.Player.Enable();
                    else
                        _playerInput.Player.Disable();
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