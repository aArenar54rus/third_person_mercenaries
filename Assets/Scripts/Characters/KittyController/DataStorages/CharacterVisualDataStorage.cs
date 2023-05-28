using System;
using UnityEngine;


namespace CatSimulator.Character
{
    [Serializable]
    public class CharacterVisualDataStorage
    {
        [SerializeField] private SkinnedMeshRenderer[] characterBodyLods = default;


        public SkinnedMeshRenderer[] CharacterBodyLods => characterBodyLods;
    }
}