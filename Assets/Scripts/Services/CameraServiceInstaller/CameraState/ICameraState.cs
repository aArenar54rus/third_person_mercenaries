using Cinemachine;
using UnityEngine;


namespace Arenar.CameraService
{
    public interface ICameraState
    {
        bool IsActive { get; }


        void SetStateActive(Camera camera, SerializableDictionary<CinemachineCameraType, CinemachineVirtualCamera> cinemachineVirtualCameras, Transform followTarget, Transform lookAtTarget);

        void OnUpdate();
        
        void OnLateUpdate();

        void SetStateDeactive();
    }
}