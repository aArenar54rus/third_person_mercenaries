using System;
using UnityEngine;
using UnityEngine.Serialization;


namespace Arenar
{
    [Serializable]
    public abstract class WeaponData
    {
        [SerializeField]
        private SerializableDictionary<ItemRarity, float> itemDamageMultiplierByRarity;
                
        [FormerlySerializedAs("_bulletPhysicalMight"),Space(10)]
        [SerializeField]
        private float _physicalMight = default;
        [SerializeField]
        private float _defaultDamage = default;
        [SerializeField]
        private float _minDamageByLvl = default;
        [SerializeField]
        private float _maxDamageByLvl = default;
        
        [Space(5), Header("Attack speed")]
        [SerializeField]
        private float attackSpeed;

        
        public SerializableDictionary<ItemRarity, float> ItemDamageMultiplierByRarity =>
            itemDamageMultiplierByRarity;
        public float PhysicalMight => _physicalMight;
        public float DefaultDamage => _defaultDamage;
        public float BulletLevelMinDamage => _minDamageByLvl;
        public float BulletLevelMaxDamage => _maxDamageByLvl;
        public float AttackSpeed => attackSpeed;
    }
}