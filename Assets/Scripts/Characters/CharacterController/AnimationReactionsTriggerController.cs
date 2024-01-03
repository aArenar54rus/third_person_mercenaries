using System;
using UnityEngine;


namespace Arenar.Character
{
    public class AnimationReactionsTriggerController : MonoBehaviour
    {
        public event Action onFootStep;
        public event Action onCompleteAction;


        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                onFootStep?.Invoke();
            }
        }

        private void CompleteAction()
        {
            onCompleteAction?.Invoke();
        }
    }
}