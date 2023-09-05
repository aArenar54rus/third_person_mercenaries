using UnityEngine.InputSystem;

namespace Arenar.Services.PlayerInputService
{
    public interface IPlayerInputService
    {
        public IInputActionCollection InputActionCollection { get; }
        
        
        void SetNewInputControlType(InputActionMapType type, bool status);
    }
}
