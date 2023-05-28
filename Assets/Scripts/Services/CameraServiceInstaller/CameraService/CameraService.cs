using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Zenject;


namespace CatSimulator.CameraService
{
    public class CameraService : ICameraService, ITickable, ILateTickable
    {
        public Camera GameCamera { get; private set; }
        public CinemachineVirtualCamera CinemachineVirtualCamera { get; private set; }
        public Dictionary<Type, ICameraState> CameraStates { get; private set; }

        private ICameraState lastActiveState;
        private bool isInitialize = false;


        [Inject]
        public void Construct(Camera camera, CinemachineVirtualCamera cinemachineVirtualCamera, ICameraState[] states, CameraStatesFactory cameraStatesFactory)
        {
            GameCamera = camera;
            CinemachineVirtualCamera = cinemachineVirtualCamera;
            
            Type baseType = typeof(ICameraState);
            Type[] implementations = baseType.GetImplementations();

            CameraStates = new Dictionary<Type, ICameraState>();
            foreach (Type cameraStateType in implementations)
            {
                var state = cameraStatesFactory.Create(cameraStateType);
                CameraStates.Add(cameraStateType, state);
            }

            isInitialize = true;
        }
        
        
        public void SetCameraState<TCameraState>(Transform followTarget, Transform lookAtTarget)
            where TCameraState : ICameraState
        {
            TCameraState currentState = (TCameraState)CameraStates[typeof(TCameraState)];

            if (lastActiveState != null)
                lastActiveState.SetStateDeactive();
            currentState.SetStateActive(GameCamera, CinemachineVirtualCamera, followTarget, lookAtTarget);
            lastActiveState = currentState;
        }

        public void Tick()
        {
            if (lastActiveState == null)
                return;
            
            lastActiveState.OnUpdate();
        }

        public void LateTick()
        {
            if (lastActiveState == null)
                return;
            
            lastActiveState.OnLateUpdate();
        }
    }
}