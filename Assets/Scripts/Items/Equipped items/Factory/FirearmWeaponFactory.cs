using Arenar.Services.InventoryService;
using UnityEngine;
using Zenject;


namespace Arenar.Character
{
    public class FirearmWeaponFactory : IEquipItemFactory<FirearmWeapon>
    {
        private readonly DiContainer container;
        private readonly InitializableManager initializableManager;
        private ItemProjectileSpawner projectileSpawner;
        
        
        public FirearmWeaponFactory(DiContainer container, InitializableManager initializableManager, ItemProjectileSpawner projectileSpawner)
        {
            this.container = container;
            this.initializableManager = initializableManager;
            this.projectileSpawner = projectileSpawner;
        }
        
        
        public FirearmWeapon Create(InventoryItemCellData equippedWeapon, Transform handOwner)
        {
            DiContainer subContainer = container.CreateSubContainer();
            FirearmWeapon weapon = GameObject.Instantiate(
                Resources.Load<GameObject>("Prefabs/Items/" + equippedWeapon.itemInventoryData.Id), handOwner)
                .GetComponent<FirearmWeapon>();
            
            /*switch (weapon)
            {
                case PistolFirearmWeapon pistol:
                    weaponObject 
                    break;
            }*/

            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.Euler(weapon.LocalRotation);
            
            weapon.InitializeWeapon(equippedWeapon.itemInventoryData);
            
            subContainer.ResolveRoots();
            subContainer.Rebind<InitializableManager>()
                .FromInstance(initializableManager)
                .NonLazy();

            subContainer.Rebind<ItemProjectileSpawner>()
                .FromInstance(projectileSpawner);
            
            subContainer.Inject(weapon);
            
            return weapon;
        }
    }
}