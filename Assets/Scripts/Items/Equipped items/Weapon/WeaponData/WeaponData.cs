using System;
using UnityEngine;


namespace Arenar
{
    [Serializable]
    public abstract class WeaponData
    {
        [SerializeField] private float damageMultiplierByItemLevel;
        [SerializeField] private SerializableDictionary<ItemRarity, float> itemDamageMultiplierByRarity;
                
        [Space(10)]
        [SerializeField] private float _bulletPhysicalMight = default;
        [SerializeField] private float _defaultDamage = default;
        [SerializeField] private float _minDamageByLvl = default;
        [SerializeField] private float _maxDamageByLvl = default;
        
        [Space(5), Header("Attack speed")]
        [SerializeField] private float attackSpeed;

        
        public float DamageMultiplierByItemLevel => damageMultiplierByItemLevel;
        public SerializableDictionary<ItemRarity, float> ItemDamageMultiplierByRarity =>
            itemDamageMultiplierByRarity;
        public float BulletPhysicalMight => _bulletPhysicalMight;
        public float DefaultDamage => _defaultDamage;
        public float BulletLevelMinDamage => _minDamageByLvl;
        public float BulletLevelMaxDamage => _maxDamageByLvl;
        public float AttackSpeed => attackSpeed;
    }
}