using Arenar.Character;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace Arenar.Items
{
    public abstract class FirearmWeapon : MonoBehaviour, IWeapon, ILevelItem
    {
        [SerializeField] protected Transform gunMuzzleTransform;
        [SerializeField] protected Transform clipTransform;
        [SerializeField] protected Transform secondHandPoint;
        [SerializeField] protected Vector3 rotationInHands;
        [SerializeField] protected LineRenderer lineRendererEffect;
        [SerializeField] protected FirearmWeaponCameraRecoilComponent firearmWeaponCameraRecoilComponent;
        
        private Tween _timeBetweenShotsTween;

        private bool _isBetweenShotsLock = false;


        public abstract FirearmWeaponClass FirearmWeaponClass { get; }
        public Transform GunMuzzleTransform => gunMuzzleTransform;
        public LineRenderer LineRendererEffect => lineRendererEffect;
        public int ItemLevel { get; protected set; }
        public ICharacterEntity ItemOwner { get; protected set; }
        public FirearmWeaponItemData FirearmWeaponItemData
        {
            get;
            protected set;
        }

        public IFirearmWeaponAttackItemComponent FirearmWeaponAttackComponent
        {
            get;
            protected set;
        }

        public IClipComponent ClipComponent
        {
            get;
            protected set;
        }

        public IEquippedItemAimComponent AimComponent
        {
            get;
            protected set;
        }
        public UnityEngine.Vector3 RotationInHands => rotationInHands;
        public float Damage
        { 
            get
            {
                int damage = (int) FirearmWeaponItemData.FirearmWeaponData.DefaultDamage;
                for (int i = 0; i < ItemLevel; i++)
                    damage += Random.Range(
                        (int)FirearmWeaponItemData.FirearmWeaponData.BulletLevelMinDamage, (int)FirearmWeaponItemData.FirearmWeaponData.BulletLevelMaxDamage);

                return damage;
            } 
        }

        public float ReloadSpeed => FirearmWeaponItemData.FirearmWeaponData.DefaultReloadSpeed;
        public int ClipSizeMax => ClipComponent.ClipSizeMax;

        public int ClipSize => ClipComponent.ClipSize;

        public float TimeBetweenAttacks => FirearmWeaponItemData.FirearmWeaponData.TimeBetweenShots;
        
        public ItemData ItemData { get; private set; }

        public WeaponType WeaponType => WeaponType.Firearm;

        public bool IsAutomaticAction => FirearmWeaponItemData.FirearmWeaponData.IsAutomaticShoot;
        
        public Transform SecondHandPoint => secondHandPoint;
        
        public bool IsShootLock => _isBetweenShotsLock;

        private Vector3 RecoilShakeDirection =>
            new Vector3(Random.Range(-1.0f, 1.0f), 1.0f, 0.0f) / 100.0f * FirearmWeaponItemData.FirearmWeaponData.RecoilShakeDefaultValue;

        private float BulletPhysicalMight
        {
            get;
            set;
        } = 0;

        public void CompleteReloadClip()
        {
            ClipComponent.Reload();
        }

        public void InitializeItem(ItemData itemData,
                                   Dictionary<System.Type, IEquippedItemComponent> itemComponents)
        {
            if (itemData is not FirearmWeaponItemData weaponInventoryItemData
                || itemData.ItemType != ItemType.FirearmWeapon)
            {
                Debug.LogError($"You try initialize weapon as {itemData.ItemType}. Check your code!");
                return;
            }

            FirearmWeaponItemData = weaponInventoryItemData;
            ItemData = itemData;
            
            FirearmWeaponAttackComponent = GetEquippedComponent<IFirearmWeaponAttackItemComponent>(itemComponents);
            ClipComponent = GetEquippedComponent<IClipComponent>(itemComponents);
            AimComponent = GetEquippedComponent<IEquippedItemAimComponent>(itemComponents);
            
            BulletPhysicalMight = weaponInventoryItemData.FirearmWeaponData.PhysicalMight;
        }
        
        public void PickUpItem(ICharacterEntity characterOwner)
        {
            ItemOwner = characterOwner;
        }

        public void DropItem()
        {
            ItemOwner = null;
        }

        public void SetItemLevel(int itemLevel) =>
            ItemLevel = itemLevel;

        public virtual void MakeShot(Vector3 direction, int addedDamageByCharacter, bool isInfinityClip = false)
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

            SetAimStatus(false);
            InitializeBullets(direction, addedDamageByCharacter);

            if (firearmWeaponCameraRecoilComponent != null)
                firearmWeaponCameraRecoilComponent.ApplyShootRecoil(RecoilShakeDirection);

            ClipComponent.ClipSize--;
            
            _isBetweenShotsLock = true;
            _timeBetweenShotsTween = DOVirtual.DelayedCall(TimeBetweenAttacks,() => _isBetweenShotsLock = false);
        }

        public void SetAimStatus(bool status)
        {
            if (AimComponent != null)
                AimComponent.SetAimStatus(status);
        }

        public void SetLaserPosition(Vector3 position)
        {
            if (AimComponent != null)
                AimComponent.OnUpdate(position);
        }

        protected virtual void InitializeBullets(Vector3 direction, int damageByCharacter = 0)
        {
            CreateBullet(direction, damageByCharacter);
        }
        
        protected virtual int GetClipSizeMax()
        {
            return (int) FirearmWeaponItemData.FirearmWeaponData.ClipSizeMax;
        }
        
        protected void CreateBullet(Vector3 direction, int damageByCharacter = 0)
        {
            DamageData damageData = new DamageData(ItemOwner, (int)Damage,
                damageByCharacter, FirearmWeaponItemData.FirearmWeaponData.GetStunPoints(),
                direction * BulletPhysicalMight);
            
            FirearmWeaponAttackComponent.MakeShoot(gunMuzzleTransform, direction, damageData);
        }
        
        protected T GetEquippedComponent<T>(Dictionary<System.Type, IEquippedItemComponent>  components)
            where T : IEquippedItemComponent
        {
            if (components.ContainsKey(typeof(T)))
                return (T)components[typeof(T)];
            
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