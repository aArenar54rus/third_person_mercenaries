using System;
using UnityEngine;


namespace CatSimulator
{
    [Serializable]
    public class PlayerCharacterParametersData
    {
        [Space(10), Header("Movement")]
        [Tooltip("Move speed of the character in m/s")]
        [SerializeField] private float moveSpeed = 2.0f;
        [Tooltip("Sprint speed of the character in m/s")]
        [SerializeField] private float sprintSpeed = 5.335f;
        [Tooltip("How fast the character turns to face movement direction"), Range(0.0f, 0.3f)]
        [SerializeField] private float rotationSmoothTime = 0.12f;
        [Tooltip("Acceleration and deceleration")]
        [SerializeField] private float speedChangeRate = 10.0f;
        
        [Space(5)]
        [Tooltip("The height the player can jump")]
        [SerializeField] private float jumpHeight = 1.2f;
        [Tooltip("The character uses its own gravity value.")]
        [SerializeField] private float gravity = -15.0f;

        [Space(5)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        [SerializeField] private float jumpTimeout = 0.50f;
        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        [SerializeField] private float fallTimeout = 0.15f;
        
        [Space(10), Header("Camera")]
        [Tooltip("For locking the camera position on all axis")]
        [SerializeField] private bool lockCameraPosition = false;
        [Tooltip("How far in degrees can you move the camera up")]
        [SerializeField] private float topClamp = 70.0f;
        [Tooltip("How far in degrees can you move the camera down")]
        [SerializeField] private float bottomClamp = -30.0f;
        
        [Space(10), Header("Ground parameters")]
        [Tooltip("Useful for rough ground")]
        [SerializeField] private float groundedOffset = default;
        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        [SerializeField] private float groundedRadius = default;
        [Tooltip("What layers the character uses as ground")]
        [SerializeField] private LayerMask groundedLayers = default;
        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        [SerializeField] private float cameraAngleOverride = 0.0f;
        
        
        public float MoveSpeed => moveSpeed;
        public float SprintSpeed => sprintSpeed;
        public float RotationSmoothTime => rotationSmoothTime;
        public float SpeedChangeRate => speedChangeRate;
        
        
        public float JumpHeight => jumpHeight;
        public float Gravity => gravity;
        public float JumpTimeout =>jumpTimeout;
        public float FallTimeout => fallTimeout;
        public float CameraAngleOverride => cameraAngleOverride;


        public bool LockCameraPosition => lockCameraPosition;
        public float TopClamp => topClamp;
        public float BottomClamp => bottomClamp;
        
        
        public float GroundedOffset => groundedOffset;
        public float GroundedRadius => groundedRadius;
        public LayerMask GroundedLayers => groundedLayers;
    }
}