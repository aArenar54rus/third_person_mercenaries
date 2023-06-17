using System;
using StarterAssets;
using UnityEngine;


namespace Arenar.Character
{
    [Serializable]
    public class CharacterPhysicsDataStorage
    {
        [SerializeField] private Transform characterTransform = default;
        [SerializeField] private Transform cameraTransform = default;
        [SerializeField] private CharacterController characterController = default;
        [SerializeField] private Transform characterCenterPoint = default;


        public Transform CharacterTransform => characterTransform;
        public Transform CameraTransform => cameraTransform;
        public CharacterController CharacterController => characterController;
        public Transform CharacterCenterPoint => characterCenterPoint;
    }
}