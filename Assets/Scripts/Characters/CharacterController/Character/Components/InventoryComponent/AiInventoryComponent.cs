using Arenar.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Arenar.Character
{
    public class AiInventoryComponent : IInventoryComponent
    {

        public void Initialize()
        {
            
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

        public FirearmWeapon CurrentActiveFirearmWeapon { get; }
        public FirearmWeapon[] EquippedFirearmWeapons { get; }

        
        public void ChangeActiveWeapon(int index)
        {
            throw new System.NotImplementedException();
        }
        
        public void AddEquippedFirearmWeapon(ItemInventoryData itemInventoryData, int orderIndex)
        {
            throw new System.NotImplementedException();
        }
    }
}