using UnityEngine;


namespace Arenar
{
    public abstract class FirearmWeapon : MonoBehaviour, IWeapon, ILevelItem
    {
        [SerializeField] protected Transform gunMuzzleTransform;
        [SerializeField] protected Transform clipTransform;
        [SerializeField] protected FirearmWeaponData firearmWeaponData;
        [SerializeField] protected Transform secondHandPoint;
        [SerializeField] protected Vector3 localRotation = new Vector3(-15, 90, -90);

        protected ItemProjectileSpawner projectileSpawner;
        protected int currentClipSize;


        public int ItemLevel { get; protected set; }
        
        public float Damage { get; protected set; }
        
        public int ClipSizeMax { get; private set; }
        
        public int ClipSize { get; private set; }
        public ItemData ItemData { get; private set; }

        public WeaponType WeaponType => WeaponType.Firearm;
        
        public bool IsAutomaticAction => firearmWeaponData.IsAutomaticShoot;
        
        public float ProjectileSpeed { get; protected set; }

        public Transform SecondHandPoint => secondHandPoint;

        public Vector3 LocalRotation => localRotation;


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

        public void InitializeWeapon(ItemData itemData)
        {
            if (itemData.ItemType != ItemType.Weapon)
            {
                Debug.LogError($"You try initialize weapon as {itemData.ItemType}. Check your code, asshole!");
                return;
            }

            ItemData = itemData;
            
            Damage = CalculateWeaponDamage();
            ClipSizeMax = GetClipSizeMax();
            ClipSize = ClipSizeMax;
            ProjectileSpeed = GetProjectileSpeed();
        }

        public void WeaponAction()
        {
            if (ClipSize > 0)
                MakeShot();
        }

        public void SetItemLevel(int itemLevel) =>
            ItemLevel = itemLevel;

        protected virtual void MakeShot()
        {
            var projectile = projectileSpawner.GetItemProjectile(firearmWeaponData.ProjectileType);
            projectile.Initialize(gunMuzzleTransform.position, gunMuzzleTransform.localRotation.eulerAngles, ProjectileSpeed, Damage);
            currentClipSize--;
        }

        protected virtual float CalculateWeaponDamage()
        {
            float multiplierByLevel = Mathf.Clamp(firearmWeaponData.DamageMultiplierByItemLevel * ItemLevel, 1, float.MaxValue);
            return firearmWeaponData.BaseDamage
                   * multiplierByLevel
                   * firearmWeaponData.ItemDamageMultiplierByRarity[ItemData.ItemRarity];
        }

        protected virtual int GetClipSizeMax()
        {
            return firearmWeaponData.ClipSizeMax;
        }

        protected virtual float GetProjectileSpeed()
        {
            return firearmWeaponData.ProjectileSpeed;
        }
    }
}