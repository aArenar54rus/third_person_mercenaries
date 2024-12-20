using RootMotion.Dynamics;
using System;
using UnityEngine;

namespace Arenar.Character
{
    [Serializable]
    public class CharacterPhysicsDataStorage
    {
        [SerializeField] private Transform characterTransform = default;
        [SerializeField] private Transform cameraTransform = default;
        [SerializeField] private CharacterController characterController = default;
        
        [Space(10), Header("Puppet Master")]
        [SerializeField] private PuppetMaster puppetMaster = default;
        [SerializeField] private SerializableDictionary<string, BehaviourBase> puppetBehaviours = new SerializableDictionary<string, BehaviourBase>();
        
        [Space(5), Header("Transform points")]
        [SerializeField] private Transform characterCenterPoint = default;
        [SerializeField] private Transform rightHandPoint = default;
        [SerializeField] private Transform leftHandPoint = default;


        public Transform CharacterTransform => characterTransform;
        public Transform CameraTransform => cameraTransform;
        public CharacterController CharacterController => characterController;
        public PuppetMaster PuppetMaster => puppetMaster;
        public SerializableDictionary<string, BehaviourBase> PuppetBehaviours => puppetBehaviours;
        public Transform CharacterCenterPoint => characterCenterPoint;
        public Transform RightHandPoint => rightHandPoint;
        public Transform LeftHandPoint => leftHandPoint;
    }
}