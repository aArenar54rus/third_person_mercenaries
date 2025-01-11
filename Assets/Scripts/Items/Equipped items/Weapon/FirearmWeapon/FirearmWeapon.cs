using Arenar.Character;
using Arenar.Services.DamageNumbersService;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;


namespace Arenar
{
    public abstract class FirearmWeapon : MonoBehaviour, IWeapon, ILevelItem
    {
        [SerializeField] protected Transform gunMuzzleTransform;
        [SerializeField] protected Transform clipTransform;
        [SerializeField] protected Transform secondHandPoint;
        [SerializeField] protected Vector3 localRotation = new Vector3(-15, 90, -90);
        [SerializeField] protected LineRenderer lineRendererEffect;
        [SerializeField] protected FirearmWeaponCameraRecoilComponent firearmWeaponCameraRecoilComponent;
        
        [Inject] public EffectsSpawner effectsSpawner;



        private bool _isBetweenShotsLock = false;


        public Transform GunMuzzleTransform =>
            gunMuzzleTransform;

        public ICharacterEntity WeaponOwner { get; protected set; }
        
        public WeaponInventoryItemData WeaponInventoryItemData
        {
            get;
            private set;
        }

        public IFirearmWeaponAttackItemComponent AttackComponent
        {
            get;
            private set;
        }

        protected IClipComponent ClipComponent
        {
            get;
            set;
        }
        
        public int ItemLevel { get; protected set; }

        public float Damage
        { 
            get
            {
                int damage = (int) WeaponInventoryItemData.FirearmWeaponData.DefaultDamage;
                for (int i = 0; i < ItemLevel; i++)
                    damage += Random.Range(
                        (int)WeaponInventoryItemData.FirearmWeaponData.BulletLevelMinDamage, (int)WeaponInventoryItemData.FirearmWeaponData.BulletLevelMaxDamage);

                return damage;
            } 
        }

        public float ReloadSpeed => WeaponInventoryItemData.FirearmWeaponData.DefaultReloadSpeed;
        public int ClipSizeMax => ClipComponent.ClipSizeMax;

        public int ClipSize => ClipComponent.ClipSize;

        public float TimeBetweenShots => WeaponInventoryItemData.FirearmWeaponData.TimeBetweenShots;
        
        public ItemInventoryData ItemInventoryData { get; private set; }

        public WeaponType WeaponType => WeaponType.Firearm;

        public bool IsAutomaticAction => WeaponInventoryItemData.FirearmWeaponData.IsAutomaticShoot;

        public bool IsFullClipReload => WeaponInventoryItemData.FirearmWeaponData.IsFullClipReload;
        
        public Transform SecondHandPoint => secondHandPoint;

        public Vector3 LocalRotation => localRotation;

        public bool IsShootLock => _isBetweenShotsLock;

        private Vector3 RecoilShakeDirection =>
            new Vector3(Random.Range(-1.0f, 1.0f), 1.0f, 0.0f) / 100.0f * WeaponInventoryItemData.FirearmWeaponData.RecoilShakeDefaultValue;

        private float BulletPhysicalMight
        {
            get;
            set;
        } = 0;

        public void ReloadClip()
        {
            ClipComponent.Reload();
        }

        public void InitializeWeapon(ItemInventoryData itemInventoryData, List<IEquippedItemComponent> itemComponents)
        {
            if (itemInventoryData is not WeaponInventoryItemData weaponInventoryItemData
                || itemInventoryData.ItemType != ItemType.Weapon)
            {
                Debug.LogError($"You try initialize weapon as {itemInventoryData.ItemType}. Check your code, asshole!");
                return;
            }

            WeaponInventoryItemData = weaponInventoryItemData;
            ItemInventoryData = itemInventoryData;
            
            AttackComponent = GetEquippedComponent<IFirearmWeaponAttackItemComponent>(itemComponents);
            ClipComponent = GetEquippedComponent<IClipComponent>(itemComponents);
            
            BulletPhysicalMight = weaponInventoryItemData.FirearmWeaponData.BulletPhysicalMight;
        }

        public void SetWeaponLevel(int level)
        {
            ItemLevel = level;
        }

        public void TakeWeaponInHand(ICharacterEntity weaponOwner)
        {
            WeaponOwner = weaponOwner;
        }

        public void SetItemLevel(int itemLevel) =>
            ItemLevel = itemLevel;

        public virtual void MakeShot(Vector3 direction, bool isInfinityClip = false)
        {
            if (_isBetweenShotsLock)
            {
                return;
            }
            
            if (ClipSize <= 0 && !isInfinityClip)
            {
                Debug.LogError("EmptyClip");
                return;
            }

            SetLaserStatus(false);
            InitializeBullets(direction);

            if (firearmWeaponCameraRecoilComponent != null)
                firearmWeaponCameraRecoilComponent.ApplyShootRecoil(RecoilShakeDirection);

            ClipComponent.ClipSize--;
            
            _isBetweenShotsLock = true;
            _timeBetweenShotsTween = DOVirtual.DelayedCall(TimeBetweenShots,() => _isBetweenShotsLock = false);
        }

        public void SetLaserStatus(bool status)
        {
            if (lineRendererEffect != null)
                lineRendererEffect.gameObject.SetActive(status);
        }

        public void SetLaserPosition(Vector3 position)
        {
            if (lineRendererEffect != null)
                lineRendererEffect.SetPosition(1, position);
        }

        protected virtual void InitializeBullets(Vector3 direction)
        {
            CreateBullet(direction);
        }
        
        protected virtual int GetClipSizeMax()
        {
            return (int) WeaponInventoryItemData.FirearmWeaponData.ClipSizeMax;
        }
        
        protected void CreateBullet(Vector3 direction)
        {
            DamageData damageData = new DamageData(WeaponOwner, (int)Damage, direction * BulletPhysicalMight);
            AttackComponent.MakeShoot(gunMuzzleTransform, direction, damageData);
        }


        protected T GetEquippedComponent<T>(List<IEquippedItemComponent> components)
            where T : IEquippedItemComponent
        {
            foreach (var component in components)
            {
                if (component is T neededComponent)
                    return neededComponent;
            }
            
            Debug.LogError($"No component of type {typeof(T)}");
            return default;
        }

        private void Update()
        {
            if (lineRendererEffect != null)
                lineRendererEffect.SetPosition(0, gunMuzzleTransform.position);
        }
    }
}