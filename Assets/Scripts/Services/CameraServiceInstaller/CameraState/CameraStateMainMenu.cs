using Cinemachine;
using UnityEngine;


namespace Arenar.CameraService
{
    public class CameraStateMainMenu : CameraState
    {
        private Camera _camera;
        
        public override void SetStateActive(Camera camera, SerializableDictionary<CinemachineCameraType, CinemachineVirtualCamera> cinemachineVirtualCameras, Transform followTarget, Transform lookAtTarget)
        {
            base.SetStateActive(camera, cinemachineVirtualCameras, followTarget, lookAtTarget);
            _camera = camera;
            _camera.transform.position = Vector3.zero;
            _camera.transform.rotation = Quaternion.Euler(Vector3.zero);

            foreach (var cinemachine in cinemachineVirtualCameras)
            {
                cinemachine.Value.enabled = false;
            }
        }

        public override void OnUpdate()
        {
            _camera.transform.position = Vector3.zero;
            _camera.transform.rotation = Quaternion.Euler(Vector3.zero);
        }

        public override void OnLateUpdate()
        {
            _camera.transform.position = Vector3.zero;
            _camera.transform.rotation = Quaternion.Euler(Vector3.zero);
        }
    }
}