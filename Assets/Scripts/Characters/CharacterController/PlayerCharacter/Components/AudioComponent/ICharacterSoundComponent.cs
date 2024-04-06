using System;
using System.Collections;
using System.Collections.Generic;
using Arenar.Character;
using UnityEngine;


namespace Arenar.Character
{
    public interface ICharacterSoundComponent : ICharacterComponent
    {
        void StopSound();
    }
    
    
    public interface ICharacterSoundComponent<TSoundType>
        : ICharacterSoundComponent
        where TSoundType : Enum
    {
        void PlaySound(TSoundType soundType);
    }
}