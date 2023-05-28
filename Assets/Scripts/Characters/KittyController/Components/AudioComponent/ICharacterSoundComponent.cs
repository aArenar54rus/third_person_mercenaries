using System;
using System.Collections;
using System.Collections.Generic;
using CatSimulator.Character;
using UnityEngine;


namespace CatSimulator.Character
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