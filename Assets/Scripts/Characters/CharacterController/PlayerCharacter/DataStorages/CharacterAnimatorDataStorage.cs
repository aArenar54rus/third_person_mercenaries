using System;
using UnityEngine;
using UnityEngine.Serialization;


namespace Arenar.Character
{
    [Serializable]
    public class CharacterAnimatorDataStorage
    {
        [SerializeField]
        private Animator animator = default;
        [SerializeField]
        private AnimationReactionsTriggerController _animationReactionsTriggerController;
        
        [Space(5), Header("Animations and triggers")]
        [SerializeField]
        private string speedAnimationName = "Speed";
        [SerializeField]
        private string groundedAnimationName = "Grounded";
        [SerializeField]
        private string jumpAnimationName = "Jump";
        [SerializeField]
        private string freeFallAnimationName = "FreeFall";
        [SerializeField]
        private string aimAnimationName = "IsAim";
        [SerializeField]
        private string motionSpeedAnimationXName = "MotionSpeedX";
        [SerializeField]
        private string motionSpeedAnimationYName = "MotionSpeedY";
        [SerializeField]
        private string handPistolAnimationName = "HandPistol";
        [SerializeField]
        private string isHandShotgunAnimationKey = "HandShotgun";
        [SerializeField]
        private string isHandRilfeAnimationKey = "HandRifle";
        [SerializeField]
        private string shootAnimationName = "Shoot";
        [SerializeField]
        private string reloadAnimationTriggerName = "Reload";

        [Space(10), Header("Parameters")]
        [SerializeField]
        private bool isReloadByAnimation = false;

        
        public Animator Animator => animator;
        public AnimationReactionsTriggerController AnimationReactionsTriggerController =>
            _animationReactionsTriggerController;
        public string SpeedAnimationName => speedAnimationName;
        public string GroundedAnimationName => groundedAnimationName;
        public string JumpAnimationName => jumpAnimationName;
        public string FreeFallAnimationName => freeFallAnimationName;
        public string AimAnimationName => aimAnimationName;
        public string MotionSpeedAnimationXName => motionSpeedAnimationXName;
        public string MotionSpeedAnimationYName => motionSpeedAnimationYName;
        public string HandPistolAnimationNam => handPistolAnimationName;
        public string IsHandShotgunAnimationKey => isHandShotgunAnimationKey;
        public string IsHandRifleAnimationKey => isHandRilfeAnimationKey;
        public string ShootAnimationName => shootAnimationName;
        public string ReloadAnimationTriggerName => reloadAnimationTriggerName;
        public bool IsReloadByAnimation => isReloadByAnimation;
    }
}