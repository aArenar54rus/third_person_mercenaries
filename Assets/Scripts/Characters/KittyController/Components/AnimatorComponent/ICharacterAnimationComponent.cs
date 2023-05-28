using System;


namespace CatSimulator.Character
{
    public interface ICharacterAnimationComponent : ICharacterComponent { }
    
    
    public interface ICharacterAnimationComponent<TAnimationType, TAnimationValue>
        : ICharacterAnimationComponent
        where TAnimationType : Enum
        where TAnimationValue : Enum
    {
        void PlayAnimation(TAnimationType animationType);

        void SetAnimationValue(TAnimationValue animationValue, float value);
    }
}