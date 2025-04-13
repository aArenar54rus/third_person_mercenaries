using System;

namespace Arenar.Character
{
    public interface ICharacterAttackComponent : ICharacterComponent
    {
        event Action onReloadStart; 
        event Action onReloadEnd; 
        event Action<float, float> onReloadProgress;
        event Action<int, int> onUpdateWeaponClipSize;
        
        
        int CharacterDamage { get; set; }
        
        bool IsInProcess { get; }


        void PlayAction();
        
        void CompleteAction();
        
        void MakeReload();
    }
}