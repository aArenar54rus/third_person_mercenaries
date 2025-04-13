using Arenar.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Arenar.Character
{
    public class CharacterDisableAimComponent : ICharacterAimComponent
    {
        public bool IsAim { get; set; }
        public float AimProgress { get; private set; }
        
        
        public void Initialize()
        {
            IsAim = false;
            AimProgress = 0;
        }


        public void DeInitialize()
        {
            throw new System.NotImplementedException();
        }


        public void OnActivate()
        {
            throw new System.NotImplementedException();
        }


        public void OnDeactivate()
        {
            throw new System.NotImplementedException();
        }



    }
}