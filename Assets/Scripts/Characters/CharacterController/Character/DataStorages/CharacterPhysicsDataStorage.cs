using RootMotion.Dynamics;
using System;
using UnityEngine;
using UnityEngine.AI;


namespace Arenar.Character
{
    [Serializable]
    public class CharacterPhysicsDataStorage
    {
        [SerializeField] private Transform characterTransform = default;
        [SerializeField] private Transform cameraTransform = default;
        [SerializeField] private CharacterController characterController = default;
        [SerializeField] private NavMeshAgent navMeshAgent = default;
        
        [Space(10), Header("Puppet Master")]
        [SerializeField] private PuppetMaster puppetMaster = default;
        [SerializeField] private SerializableDictionary<string, BehaviourBase> puppetBehaviours = new SerializableDictionary<string, BehaviourBase>();
        
        [Space(5), Header("Transform points")]
        [SerializeField] private Transform characterCenterPoint = default;
        
        [Space(5), Header("Hands")]
        [SerializeField] private CharacterHandPoint handRight = default;
        [SerializeField] private CharacterHandPoint handLeft = default;
        
        [Space(5), Header("Damage containers")]
        [SerializeField]
        private CharacterDamageContainer[] damageContainers;


        public Transform CharacterTransform => characterTransform;
        public Transform CameraTransform => cameraTransform;
        public NavMeshAgent NavMeshAgent => navMeshAgent;
        public CharacterController CharacterController => characterController;
        public PuppetMaster PuppetMaster => puppetMaster;
        public SerializableDictionary<string, BehaviourBase> PuppetBehaviours => puppetBehaviours;
        public Transform CharacterCenterPoint => characterCenterPoint;
        public CharacterHandPoint RightHandPoint => handRight;
        public CharacterHandPoint LeftHandPoint => handLeft;
        public CharacterDamageContainer[] DamageContainers => damageContainers;
    }
}