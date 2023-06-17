using Cinemachine;
using UnityEngine;


namespace Arenar.CameraService
{
    public abstract class CameraState : ICameraState
    {
        public bool IsActive { get; private set; }
        
        
        
        public virtual void SetStateActive(Camera camera, CinemachineVirtualCamera cinemachineVirtualCamera, Transform followTarget, Transform lookAtTarget)
        {
            IsActive = true;
        }

        public abstract void OnUpdate();

        public abstract void OnLateUpdate();

        public virtual void SetStateDeactive()
        {
            IsActive = false;
        }
    }
}