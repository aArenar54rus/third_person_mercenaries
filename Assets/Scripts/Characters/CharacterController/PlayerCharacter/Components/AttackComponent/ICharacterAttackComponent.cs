using System;

namespace Arenar.Character
{
    public interface ICharacterAttackComponent : ICharacterComponent
    {
        public event Action onReloadStart; 
        public event Action onReloadEnd; 
        public event Action<float, float> onReloadProgress;
        public event Action<int, int> onUpdateWeaponClipSize;


        public void CompleteAction();
    }
}