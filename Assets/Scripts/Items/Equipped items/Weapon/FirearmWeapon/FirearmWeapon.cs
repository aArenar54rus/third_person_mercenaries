using Arenar.Character;
using Arenar.Services.DamageNumbersService;
using DG.Tweening;
using UnityEngine;
using Zenject;


namespace Arenar
{
    public abstract class FirearmWeapon : MonoBehaviour, IWeapon, ILevelItem
    {
        [SerializeField] protected Transform gunMuzzleTransform;
        [SerializeField] protected Transform clipTransform;
        [SerializeField] protected FirearmWeaponData firearmWeaponData;
        [SerializeField] protected Transform secondHandPoint;
        [SerializeField] protected Vector3 localRotation = new Vector3(-15, 90, -90);
        [SerializeField] protected LineRenderer lineRendererEffect;
        [SerializeField] protected FirearmWeaponCameraRecoilComponent firearmWeaponCameraRecoilComponent;
        
        [Inject] public EffectsSpawner projectileSpawner;

        private Tween _timeBetweenShotsTween;

        private bool _isBetweenShotsLock = false;
        
        private int _defaultDamage;
        private int _minDamage;
        private int _maxDamage;


        public Transform GunMuzzleTransform =>
            gunMuzzleTransform;

        public ICharacterEntity WeaponOwner { get; protected set; }
        
        public int ItemLevel { get; protected set; }

        public float Damage
        { 
            get
            {
                int damage = _defaultDamage;
                for (int i = 0; i < ItemLevel; i++)
                    damage += Random.Range(_minDamage, _maxDamage);

                return damage;
            } 
        }

        public float ReloadSpeed { get; protected set; }

        public int ClipSizeMax { get; private set; }

        public int ClipSize { get; private set; }

        public float TimeBetweenShots { get; private set; }
        
        public ItemInventoryData ItemInventoryData { get; private set; }

        public WeaponType WeaponType => WeaponType.Firearm;

        public bool IsAutomaticAction => firearmWeaponData.IsAutomaticShoot;

        public bool IsFullClipReload => firearmWeaponData.IsFullClipReload;

        public float ProjectileSpeed { get; protected set; }

        public Transform SecondHandPoint => secondHandPoint;

        public Vector3 LocalRotation => localRotation;

        public bool IsShootLock => _isBetweenShotsLock;

        private Vector3 RecoilShakeDirection =>
            new Vector3(Random.Range(-1.0f, 1.0f), 1.0f, 0.0f) / 100.0f * firearmWeaponData.RecoilShakeDefaultValue;

        private float BulletPhysicalMight
        {
            get;
            set;
        } = 0;

        public void ReloadClip(bool isFull)
        {
            if (isFull)
            {
                ClipSize = ClipSizeMax;
            }
            else
            {
                ClipSize++;
                if (ClipSize > ClipSizeMax)
                    ClipSize = ClipSizeMax;
            }
            
            _timeBetweenShotsTween?.Kill(true);
        }

        public void InitializeWeapon(ItemInventoryData itemInventoryData)
        {
            if (itemInventoryData.ItemType != ItemType.Weapon)
            {
                Debug.LogError($"You try initialize weapon as {itemInventoryData.ItemType}. Check your code, asshole!");
                return;
            }
            
            ItemInventoryData = itemInventoryData;
            
            ClipSizeMax = GetClipSizeMax();
            ClipSize = ClipSizeMax;
            
            BulletPhysicalMight = itemInventoryData.BulletPhysicalMight;
            
            _defaultDamage = (int)itemInventoryData.DefaultDamage;
            _minDamage = (int)itemInventoryData.BulletLevelMinDamage;
            _maxDamage = (int)itemInventoryData.BulletLevelMaxDamage;
            
            ProjectileSpeed = GetProjectileSpeed();
            ReloadSpeed = firearmWeaponData.DefaultReloadSpeed;
            TimeBetweenShots = firearmWeaponData.TimeBetweenShots;
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
            PlayMuzzleFlashEffect();
            InitializeBullets(direction);

            if (firearmWeaponCameraRecoilComponent != null)
                firearmWeaponCameraRecoilComponent.ApplyShootRecoil(RecoilShakeDirection);

            if (!isInfinityClip)
                ClipSize--;
            
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
            return firearmWeaponData.ClipSizeMax;
        }

        protected virtual float GetProjectileSpeed()
        {
            return firearmWeaponData.ProjectileSpeed;
        }
        
        protected void CreateBullet(Vector3 direction)
        {
            DamageData damageData = new DamageData(WeaponOwner, (int)Damage, direction * BulletPhysicalMight);
            
            switch (firearmWeaponData.FirearmWeaponAttackType)
            {
                case FirearmWeaponAttackType.Projectile:
                    var projectile = projectileSpawner.GetItemProjectile(firearmWeaponData.EffectType);
                    projectile.Initialize(gunMuzzleTransform.position,
                        gunMuzzleTransform.rotation, 
                        direction,
                        ProjectileSpeed,
                        damageData,
                        projectileSpawner);
                    return;

                case FirearmWeaponAttackType.Raycast:
                    Ray ray = new(gunMuzzleTransform.position, direction);
                    if (!Physics.Raycast(ray, out RaycastHit hit))
                        return;
                    
                    if (hit.transform.TryGetComponent<CharacterController>(
                            out CharacterController characterController)
                        && characterController.TryGetComponent<ICharacterLiveComponent>(
                            out ICharacterLiveComponent characterLiveComponent))
                    {
                        characterLiveComponent.SetDamage(damageData);

                    }
                    
                    ParticleSystem effect = projectileSpawner.GetEffect(EffectType.BulletCollision);
                    effect.transform.position = hit.transform.position;
                    effect.Play();
                    
                    return;
                
                default:
                    Debug.LogError($"Unknown Firearm Weapon Attack Type {firearmWeaponData.FirearmWeaponAttackType}.");
                    return;
            }
        }

        private void Update()
        {
            if (lineRendererEffect != null)
                lineRendererEffect.SetPosition(0, gunMuzzleTransform.position);
        }

        private void PlayMuzzleFlashEffect()
        {
            var effect = projectileSpawner.GetEffect(EffectType.MuzzleFlashYellow);
            effect.gameObject.SetActive(true);
            effect.transform.SetParent(gunMuzzleTransform);
            effect.transform.localPosition = Vector3.zero;
            effect.transform.rotation = Quaternion.Inverse(gunMuzzleTransform.rotation);  //gunMuzzleTransform.rotation;
            effect.Play();

            DOVirtual.DelayedCall(1.0f, () =>
            {
                projectileSpawner.ReturnEffect(effect);
            });
        }
    }
}