using Cinemachine;
using UnityEngine;


namespace CatSimulator.CameraService
{
    public interface ICameraState
    {
        bool IsActive { get; }


        void SetStateActive(Camera camera, CinemachineVirtualCamera cinemachineVirtualCamera, Transform followTarget, Transform lookAtTarget);

        void OnUpdate();
        
        void OnLateUpdate();

        void SetStateDeactive();
    }
}