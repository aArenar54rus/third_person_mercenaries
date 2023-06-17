using System;
using UnityEngine;


namespace Arenar.Character
{
    public class AnimationReactionsController : MonoBehaviour
    {
        public event Action onFootStep;


        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                onFootStep?.Invoke();
            }
        }
    }
}