using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;


namespace Arenar.CameraService
{
    public interface ICameraService 
    {
        Camera GameCamera { get; }
        
        SerializableDictionary<CinemachineCameraType, CinemachineVirtualCamera> CinemachineVirtualCameras { get; }

        Dictionary<Type, ICameraState> CameraStates { get; }


        void SetCinemachineVirtualCamera(CinemachineCameraType cinemachineCameraType);
        
        void SetCameraState<TCameraState>(Transform followTarget, Transform lookAtTarget)
            where TCameraState : ICameraState;
    }
}