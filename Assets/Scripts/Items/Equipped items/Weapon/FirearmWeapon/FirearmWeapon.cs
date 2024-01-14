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
        [Inject] protected ItemProjectileSpawner projectileSpawner;


        public Transform GunMuzzleTransform =>
            gunMuzzleTransform;

        public int ItemLevel { get; protected set; }

        public float Damage { get; protected set; }

        public float ReloadSpeed { get; protected set; }

        public int ClipSizeMax { get; private set; }

        public int ClipSize { get; private set; }
        public ItemInventoryData ItemInventoryData { get; private set; }

        public WeaponType WeaponType => WeaponType.Firearm;

        public bool IsAutomaticAction => firearmWeaponData.IsAutomaticShoot;

        public bool IsFullClipReload => firearmWeaponData.IsFullClipReload;

        public float ProjectileSpeed { get; protected set; }

        public Transform SecondHandPoint => secondHandPoint;

        public Vector3 LocalRotation => localRotation;

        private Vector3 RecoilShakeDirection =>
            new Vector3(Random.Range(-1.0f, 1.0f), 1.0f, 0.0f) / 100.0f * firearmWeaponData.RecoilShakeDefaultValue;


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
        }

        public void InitializeWeapon(ItemInventoryData itemInventoryData)
        {
            if (itemInventoryData.ItemType != ItemType.Weapon)
            {
                Debug.LogError($"You try initialize weapon as {itemInventoryData.ItemType}. Check your code, asshole!");
                return;
            }

            ItemInventoryData = itemInventoryData;

            Damage = CalculateWeaponDamage();
            ClipSizeMax = GetClipSizeMax();
            ClipSize = ClipSizeMax;
            ProjectileSpeed = GetProjectileSpeed();
            ReloadSpeed = firearmWeaponData.DefaultReloadSpeed;
        }

        public void SetItemLevel(int itemLevel) =>
            ItemLevel = itemLevel;

        public virtual void MakeShot(Vector3 directional, bool isInfinityClip = false)
        {
            if (ClipSize <= 0)
            {
                Debug.LogError("EmptyClip");
                return;
            }

            SetLaserStatus(false);
            switch (firearmWeaponData.FirearmAttackType)
            {
                case ItemFirearmAttackType.Projectile:
                    var projectile = projectileSpawner.GetItemProjectile(firearmWeaponData.FirearmAttackType);
                    projectile.Initialize(gunMuzzleTransform.position, directional, ProjectileSpeed, Damage);
                    break;

                case ItemFirearmAttackType.Raycast:

                    Ray ray = new Ray(gunMuzzleTransform.position, directional);
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        Debug.LogError($"YOU hit in {hit.transform.name}");
                    }

                    break;
            }

            firearmWeaponCameraRecoilComponent.ApplyShootRecoil(RecoilShakeDirection);

            if (!isInfinityClip)
                ClipSize--;
        }

        public void SetLaserStatus(bool status)
        {
            lineRendererEffect.gameObject.SetActive(status);
        }

        public void SetLaserPosition(Vector3 position) =>
            lineRendererEffect.SetPosition(1, position);

        protected virtual float CalculateWeaponDamage()
        {
            float multiplierByLevel = Mathf.Clamp(firearmWeaponData.DamageMultiplierByItemLevel * ItemLevel, 1, float.MaxValue);
            return firearmWeaponData.BaseDamage
                   * multiplierByLevel
                   * firearmWeaponData.ItemDamageMultiplierByRarity[ItemInventoryData.ItemRarity];
        }

        protected virtual int GetClipSizeMax()
        {
            return firearmWeaponData.ClipSizeMax;
        }

        protected virtual float GetProjectileSpeed()
        {
            return firearmWeaponData.ProjectileSpeed;
        }

        private void Update()
        {
            lineRendererEffect.SetPosition(0, gunMuzzleTransform.position);
        }
    }
}