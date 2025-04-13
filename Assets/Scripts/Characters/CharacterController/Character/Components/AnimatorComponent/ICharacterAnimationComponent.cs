using System;
using UnityEngine;


namespace Arenar.Character
{
    public interface ICharacterAnimationComponent : ICharacterComponent
    {
        event Action<AnimationEvent> onAnimationEvent;
    }
    
    
    public interface ICharacterAnimationComponent<TAnimationType, TAnimationValue>
        : ICharacterAnimationComponent
        where TAnimationType : Enum
        where TAnimationValue : Enum
    {
        void PlayAnimation(TAnimationType animationType);

        void SetAnimationValue(TAnimationValue animationValue, float value);
    }
}