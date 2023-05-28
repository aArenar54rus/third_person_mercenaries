using System;
using StarterAssets;
using UnityEngine;


namespace CatSimulator.Character
{
    [Serializable]
    public class CharacterPhysicsDataStorage
    {
        [SerializeField] private Transform characterTransform = default;
        [SerializeField] private Transform cameraTransform = default;
        [SerializeField] private CharacterController characterController;


        public Transform CharacterTransform => characterTransform;
        public Transform CameraTransform => cameraTransform;
        public CharacterController CharacterController => characterController;
    }
}