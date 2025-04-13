using Arenar.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Arenar.Character
{
    public class AiInventoryComponent : IInventoryComponent
    {

        public MeleeWeapon CurrentActiveMeleeWeapon { get; private set; }
        public FirearmWeapon CurrentActiveFirearmWeapon => null;
        public FirearmWeapon[] EquippedFirearmWeapons => null;
        
        
        
        public void Initialize()
        {
            CurrentActiveMeleeWeapon
        }

        public void DeInitialize()
        {
            
        }
        
        public void OnActivate()
        {
            
        }
        
        public void OnDeactivate()
        {
            
        }
        
        public void ChangeActiveWeapon(int index)
        {
            
        }
        
        public void AddEquippedFirearmWeapon(ItemInventoryData itemInventoryData, int orderIndex)
        {
            throw new System.NotImplementedException();
        }
    }
}