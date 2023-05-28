using Cinemachine;
using UnityEngine;


namespace CatSimulator.CameraService
{
    public class CameraStateThirdPerson : CameraState
    {
        public override void SetStateActive(Camera camera, CinemachineVirtualCamera cinemachineVirtualCamera, Transform followTarget, Transform lookAtTarget)
        {
            base.SetStateActive(camera, cinemachineVirtualCamera, followTarget, lookAtTarget);
            
            cinemachineVirtualCamera.Follow = followTarget;
            cinemachineVirtualCamera.LookAt = lookAtTarget;

            cinemachineVirtualCamera.ForceCameraPosition(followTarget.position, Quaternion.Euler(lookAtTarget.position));
        }
        
        public override void OnUpdate() { }

        public override void OnLateUpdate() { }
    }
}