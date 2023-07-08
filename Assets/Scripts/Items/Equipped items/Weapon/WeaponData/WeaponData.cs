using System;
using UnityEngine;


namespace Arenar
{
    [Serializable]
    public abstract class WeaponData
    {
        [Header("Damage")]
        [SerializeField] private float baseDamage;
        [SerializeField] private float damageMultiplierByItemLevel;
        [SerializeField] private SerializableDictionary<ItemRarity, float> itemDamageMultiplierByRarity;
        
        [Space(5), Header("Attack speed")]
        [SerializeField] private float attackSpeed;


        public float BaseDamage => baseDamage;
        
        public float DamageMultiplierByItemLevel => damageMultiplierByItemLevel;

        public SerializableDictionary<ItemRarity, float> ItemDamageMultiplierByRarity =>
            itemDamageMultiplierByRarity;
            
        public float AttackSpeed => attackSpeed;
    }
}