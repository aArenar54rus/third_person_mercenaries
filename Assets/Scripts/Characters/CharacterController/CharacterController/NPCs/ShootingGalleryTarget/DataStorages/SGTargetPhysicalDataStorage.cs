using System;
using UnityEngine;


namespace Arenar.Character
{
    [Serializable]
    public class SGTargetPhysicalDataStorage
    {
        [SerializeField] private Transform _characterTransform = default;
        [SerializeField] private Rigidbody _characterModelModelRigidbody = default;
        
        
        public Transform CharacterTransform => _characterTransform;
        public Rigidbody CharacterModelRigidbody => _characterModelModelRigidbody;
    }
}