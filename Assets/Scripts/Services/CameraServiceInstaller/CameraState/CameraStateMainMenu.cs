using Cinemachine;
using UnityEngine;


namespace Arenar.CameraService
{
    public class CameraStateMainMenu : CameraState
    {
        
        public override void SetStateActive(Camera camera, SerializableDictionary<CinemachineCameraType, CinemachineVirtualCamera> cinemachineVirtualCameras, Transform followTarget, Transform lookAtTarget)
        {
            base.SetStateActive(camera, cinemachineVirtualCameras, followTarget, lookAtTarget);
            camera.transform.position = Vector3.zero;
            camera.transform.rotation = Quaternion.Euler(Vector3.zero);
        }
        
        public override void OnUpdate() { }

        public override void OnLateUpdate() { }
    }
}