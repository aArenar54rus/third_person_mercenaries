using System;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Arenar
{
    [Serializable]
    public class MeleeWeaponData : WeaponData
    {
        [SerializeField]
        private float timeBetweenAttacks = 1.0f;
        [SerializeField]
        private int stunPointMin;
        [SerializeField]
        private int stunPointMax;
        
        
        public int GetStunPoints()
        {
            return Random.Range(stunPointMin, stunPointMax);
        }
    }
}