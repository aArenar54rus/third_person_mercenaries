using Arenar.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace Arenar.Items
{
    public class MeleeWeaponFactory : IEquipItemFactory<MeleeWeapon>
    {
        private readonly DiContainer container;
        private readonly InitializableManager initializableManager;
        
        
        public MeleeWeaponFactory(DiContainer container, InitializableManager initializableManager)
        {
            this.container = container;
            this.initializableManager = initializableManager;
        }
        

        public MeleeWeapon Create(ItemData itemData)
        {
            if (itemData is not MeleeWeaponItemData weaponInventoryItemData)
            {
                Debug.LogError("Send wrong inventory Item Data");
                return null;
            }
            
            MeleeWeapon weapon = GameObject.Instantiate(
                    Resources.Load<GameObject>("Prefabs/Items/" + itemData.Id), null)
                .GetComponent<MeleeWeapon>();
            
            DiContainer subContainer = container.CreateSubContainer();
            
            subContainer.ResolveRoots();
            subContainer.Rebind<InitializableManager>()
                .FromInstance(initializableManager)
                .NonLazy();
            
            subContainer.Inject(weapon);
            
            Dictionary<Type, IEquippedItemComponent> components = new Dictionary<Type, IEquippedItemComponent> ();
            components.Add(typeof(IMeleeWeaponAttackComponent), GetMeleeAttackComponent(weaponInventoryItemData));
            
            weapon.InitializeItem(itemData, components);
            return weapon;
        }
        
        private IMeleeWeaponAttackComponent GetMeleeAttackComponent(MeleeWeaponItemData weaponItemData)
        {
            return new MeleeWeaponAttackComponent();
        }
    }
}