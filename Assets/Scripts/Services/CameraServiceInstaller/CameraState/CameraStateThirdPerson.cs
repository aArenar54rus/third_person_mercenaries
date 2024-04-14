using Cinemachine;
using UnityEngine;


namespace Arenar.CameraService
{
    public class CameraStateThirdPerson : CameraState
    {
        public override void SetStateActive(Camera camera, SerializableDictionary<CinemachineCameraType, CinemachineVirtualCamera> cinemachineVirtualCameras, Transform followTarget, Transform lookAtTarget)
        {
            base.SetStateActive(camera, cinemachineVirtualCameras, followTarget, lookAtTarget);

            foreach (var cinemachineVirtualCamera in cinemachineVirtualCameras)
            {
                cinemachineVirtualCamera.Value.enabled = true;
                cinemachineVirtualCamera.Value.Follow = followTarget;
                cinemachineVirtualCamera.Value.LookAt = lookAtTarget;

                cinemachineVirtualCamera.Value.ForceCameraPosition(followTarget.position, Quaternion.Euler(lookAtTarget.position));
            }
        }
        
        public override void OnUpdate() { }

        public override void OnLateUpdate() { }
    }
}