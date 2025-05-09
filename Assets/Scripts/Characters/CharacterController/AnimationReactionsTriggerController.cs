using System;
using UnityEngine;


namespace Arenar.Character
{
    public class AnimationReactionsTriggerController : MonoBehaviour
    {
        public event Action onFootStep;
        public event Action onCompleteAction;
        public event Action<string> onAnimationEventTriggered;


        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                Debug.Log("Footstep");
                onFootStep?.Invoke();
            }
        }

        private void CompleteAction()
        {
            onCompleteAction?.Invoke();
        }
        
        private void OnAnimationEventTriggered(string parameter)
        {
            onAnimationEventTriggered?.Invoke(parameter);
        }
    }
}