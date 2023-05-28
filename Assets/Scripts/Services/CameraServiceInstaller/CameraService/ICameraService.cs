using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;


namespace CatSimulator.CameraService
{
    public interface ICameraService 
    {
        Camera GameCamera { get; }
        
        CinemachineVirtualCamera CinemachineVirtualCamera { get; }

        Dictionary<Type, ICameraState> CameraStates { get; }


        void SetCameraState<TCameraState>(Transform followTarget, Transform lookAtTarget)
            where TCameraState : ICameraState;
    }
}