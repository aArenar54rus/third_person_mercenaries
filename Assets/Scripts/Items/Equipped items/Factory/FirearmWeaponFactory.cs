using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace Arenar.Items
{
    public class FirearmWeaponFactory : IEquipItemFactory<FirearmWeapon>
    {
        private readonly DiContainer container;
        private readonly InitializableManager initializableManager;
        private EffectsSpawner projectileSpawner;
        
        
        public FirearmWeaponFactory(DiContainer container, InitializableManager initializableManager, EffectsSpawner projectileSpawner)
        {
            this.container = container;
            this.initializableManager = initializableManager;
            this.projectileSpawner = projectileSpawner;
        }
        
        
        public FirearmWeapon Create(ItemInventoryData itemInventoryData)
        {
            if (itemInventoryData is not WeaponInventoryItemData weaponInventoryItemData)
            {
                Debug.LogError("Send wrong inventory Item Data");
                return null;
            }
            
            DiContainer subContainer = container.CreateSubContainer();
            FirearmWeapon weapon = GameObject.Instantiate(
                    Resources.Load<GameObject>("Prefabs/Items/" + itemInventoryData.Id), null)
                .GetComponent<FirearmWeapon>();

            weapon.transform.localPosition = Vector3.zero;
            // weapon.transform.localRotation = Quaternion.Euler(weapon.LocalRotation);
            
            subContainer.ResolveRoots();
            subContainer.Rebind<InitializableManager>()
                .FromInstance(initializableManager)
                .NonLazy();

            subContainer.Rebind<EffectsSpawner>()
                .FromInstance(projectileSpawner);
            
            subContainer.Inject(weapon);

            Dictionary<Type, IEquippedItemComponent> components = new Dictionary<Type, IEquippedItemComponent> ();
            
            components.Add(typeof(IFirearmWeaponAttackItemComponent),GetAttackMechanism(weaponInventoryItemData));
            components.Add(typeof(IClipComponent), GetClipComponent(weaponInventoryItemData));
            components.Add(typeof(IEquippedItemAimComponent), new LaserAimComponent(weapon.LineRendererEffect) as IEquippedItemComponent);
            
            weapon.InitializeItem(itemInventoryData, components);
            
            return weapon;
        }

        private IFirearmWeaponAttackItemComponent GetAttackMechanism(WeaponInventoryItemData weaponInventoryItemData)
        {
            switch (weaponInventoryItemData.FirearmWeaponData.FirearmWeaponAttackType)
            {
                case FirearmWeaponAttackType.Projectile:
                    return new ProjectileFirearmWeaponAttackItemComponent(
                        projectileSpawner,
                        weaponInventoryItemData.FirearmWeaponData.EffectType,
                        weaponInventoryItemData.FirearmWeaponData.ProjectileSpeed
                    );
                    break;
                
                case FirearmWeaponAttackType.Raycast:
                    return new RaycastFirearmWeaponAttackItemComponent(projectileSpawner);
                    break;
            }

            return null;
        }
        
        private IClipComponent GetClipComponent(WeaponInventoryItemData weaponInventoryItemData)
        {
            var weaponData = weaponInventoryItemData.FirearmWeaponData;
            if (weaponData.IsFullClipReload) 
                return new FirearmWeaponClipComponent(false, weaponData.ClipSizeMax, weaponData.ClipSizeMax, weaponData.DefaultReloadSpeed);
            else
                return new OneBulletReloadFirearmClipComponent(false, weaponData.ClipSizeMax, weaponData.ClipSizeMax, weaponData.DefaultReloadSpeed);
        }
    }
}