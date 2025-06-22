using System;
using UnityEngine;

namespace Arenar.Character
{
    [Serializable]
    public class EnemyCharacterDataStorage
    {
        [SerializeField]
        private EnemyCharacterParameters parameters;
        [SerializeField]
        private SerializableDictionary<ECharacterDamageContainerBodyType, Collider> _bodyColliders;
        
        
        public EnemyCharacterParameters EnemyCharacterParameters => parameters;
        public SerializableDictionary<ECharacterDamageContainerBodyType, Collider>  BodyColliders => _bodyColliders;
    }
}