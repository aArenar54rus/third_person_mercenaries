using Sirenix.OdinInspector;
using UnityEngine;

public class MouseUnlocker : MonoBehaviour
{
    [Button]
    private void LockButton()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    [Button]
    private void UnlockButton()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}
