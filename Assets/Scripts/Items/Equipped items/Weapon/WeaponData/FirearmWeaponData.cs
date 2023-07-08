using System;
using UnityEngine;


namespace Arenar
{
    [Serializable]
    public class FirearmWeaponData : WeaponData
    {
        [Space(5)]
        [SerializeField] private ItemProjectileType projectileType;
        [SerializeField] private bool isAutomaticShoot;
        [SerializeField] private int clipSizeMax;
        [SerializeField] private float defaultReloadSpeed;
        [SerializeField] private float projectileSpeed;


        public ItemProjectileType ProjectileType =>
            projectileType;
        
        public bool IsAutomaticShoot =>
            isAutomaticShoot;

        public int ClipSizeMax => clipSizeMax;
        
        public float DefaultReloadSpeed => defaultReloadSpeed;
        
        public float ProjectileSpeed => projectileSpeed;
    }
}