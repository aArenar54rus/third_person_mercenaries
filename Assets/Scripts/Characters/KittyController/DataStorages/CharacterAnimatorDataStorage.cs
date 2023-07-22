using System;
using UnityEngine;


namespace Arenar.Character
{
    [Serializable]
    public class CharacterAnimatorDataStorage
    {
        [SerializeField] private Animator animator = default;
        [SerializeField] private string speedAnimationName = "Speed";
        [SerializeField] private string groundedAnimationName = "Grounded";
        [SerializeField] private string jumpAnimationName = "Jump";
        [SerializeField] private string freeFallAnimationName = "FreeFall";
        [SerializeField] private string aimAnimationName = "IsAim";
        [SerializeField] private string motionSpeedAnimationXName = "MotionSpeedX";
        [SerializeField] private string motionSpeedAnimationYName = "MotionSpeedY";


        public Animator Animator => animator;
        public string SpeedAnimationName => speedAnimationName;
        public string GroundedAnimationName => groundedAnimationName;
        public string JumpAnimationName => jumpAnimationName;
        public string FreeFallAnimationName => freeFallAnimationName;
        public string AimAnimationName => aimAnimationName;
        public string MotionSpeedAnimationXName => motionSpeedAnimationXName;
        public string MotionSpeedAnimationYName => motionSpeedAnimationYName;
    }
}