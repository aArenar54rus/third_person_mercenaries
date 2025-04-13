using System;
using UnityEngine;

namespace Arenar.Character
{
    [Serializable]
    public class EnemyCharacterDataStorage
    {
        [SerializeField]
        private EnemyCharacterParameters parameters;
        
        
        public EnemyCharacterParameters EnemyCharacterParameters => parameters;
    }
}