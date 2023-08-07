using System;
using UnityEngine;
using UnityEngine.Serialization;


namespace Arenar
{
    [Serializable]
    public class FirearmWeaponData : WeaponData
    {
        [FormerlySerializedAs("projectileType")]
        [Space(5)]
        [SerializeField] private ItemFirearmAttackType firearmAttackType;
        [SerializeField] private bool isAutomaticShoot;
        [SerializeField] private int clipSizeMax;
        [SerializeField] private float defaultReloadSpeed;
        [SerializeField] private float projectileSpeed;


        public ItemFirearmAttackType FirearmAttackType =>
            firearmAttackType;
        
        public bool IsAutomaticShoot =>
            isAutomaticShoot;

        public int ClipSizeMax => clipSizeMax;
        
        public float DefaultReloadSpeed => defaultReloadSpeed;
        
        public float ProjectileSpeed => projectileSpeed;
    }
}