using System;
using UnityEngine;


namespace CatSimulator.Character
{
    [Serializable]
    public class CharacterAnimatorDataStorage
    {
        [SerializeField] private Animator animator = default;
        [SerializeField] private string speedAnimationName = "Speed";
        [SerializeField] private string groundedAnimationName = "Grounded";
        [SerializeField] private string jumpAnimationName = "Jump";
        [SerializeField] private string freeFallAnimationName = "FreeFall";
        [SerializeField] private string motionSpeedAnimationName = "MotionSpeed";


        public Animator Animator => animator;
        public string SpeedAnimationName => speedAnimationName;
        public string GroundedAnimationName => groundedAnimationName;
        public string JumpAnimationName => jumpAnimationName;
        public string FreeFallAnimationName => freeFallAnimationName;
        public string MotionSpeedAnimationName => motionSpeedAnimationName;
    }
}