using System;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Arenar
{
    [Serializable]
    public class FirearmWeaponData : WeaponData
    {
        [Space(5)]
        [SerializeField]
        private FirearmWeaponClass firearmWeaponClass;
        [SerializeField]
        private FirearmWeaponAttackType firearmWeaponAttackType;
        [SerializeField]
        private EffectType effectType;
        [SerializeField]
        private bool isAutomaticShoot;
        [SerializeField]
        private int clipSizeMax;
        [SerializeField]
        private float defaultReloadSpeed;
        [SerializeField]
        private float timeBetweenShots = 1.0f;
        [SerializeField]
        private bool isFullClipReload;
        [SerializeField]
        private float projectileSpeed;
        [SerializeField]
        private float recoilShakeDefaultValue;
        [SerializeField]
        private int stunPointMin;
        [SerializeField]
        private int stunPointMax;


        public FirearmWeaponClass FirearmWeaponClass =>
            firearmWeaponClass;
        
        public FirearmWeaponAttackType FirearmWeaponAttackType =>
            firearmWeaponAttackType;

        public EffectType EffectType =>
            effectType;
        
        public bool IsAutomaticShoot =>
            isAutomaticShoot;

        public int ClipSizeMax => clipSizeMax;

        public float TimeBetweenShots => timeBetweenShots;
        
        public float DefaultReloadSpeed => defaultReloadSpeed;

        public bool IsFullClipReload => isFullClipReload;
        
        public float ProjectileSpeed => projectileSpeed;

        public float RecoilShakeDefaultValue => recoilShakeDefaultValue;


        public int GetStunPoints()
        {
            return Random.Range(stunPointMin, stunPointMax);
        }
    }
}