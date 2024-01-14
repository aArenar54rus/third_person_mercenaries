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
        [SerializeField] private bool isFullClipReload;
        [SerializeField] private float projectileSpeed;
        [SerializeField] private float recoilShakeDefaultValue;


        public ItemFirearmAttackType FirearmAttackType =>
            firearmAttackType;
        
        public bool IsAutomaticShoot =>
            isAutomaticShoot;

        public int ClipSizeMax => clipSizeMax;
        
        public float DefaultReloadSpeed => defaultReloadSpeed;

        public bool IsFullClipReload => isFullClipReload;
        
        public float ProjectileSpeed => projectileSpeed;

        public float RecoilShakeDefaultValue => recoilShakeDefaultValue;
    }
}