using Arenar.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace Arenar.Character
{
    public class AiInventoryComponent : IInventoryComponent
    {
        private const int SWORD_WEAPON_INDEX = 4;
        
        
        private ICharacterEntity character;
        
        private ItemCollectionData itemCollectionData;
        
        private MeleeWeaponFactory meleeWeaponFactory;
        private FirearmWeaponFactory firearmWeaponFactory;

        private CharacterPhysicsDataStorage characterPhysicsData;
        

        public MeleeWeapon CurrentActiveMeleeWeapon { get; private set; }
        public FirearmWeapon CurrentActiveFirearmWeapon => null;
        public FirearmWeapon[] EquippedFirearmWeapons => null;


        [Inject]
        public void Construct(ICharacterEntity character,
                              MeleeWeaponFactory meleeWeaponFactory,
                              FirearmWeaponFactory firearmWeaponFactory,
                              ItemCollectionData itemCollectionData,
                              ICharacterDataStorage<CharacterPhysicsDataStorage> characterPhysicsDataStorage)
        {
            this.character = character;
            this.meleeWeaponFactory = meleeWeaponFactory;
            this.firearmWeaponFactory = firearmWeaponFactory;
            this.itemCollectionData = itemCollectionData;
            this.characterPhysicsData = characterPhysicsDataStorage.Data;
        }
        
        
        public void Initialize()
        {
            characterPhysicsData.RightHandPoint.Initialize(character);
            characterPhysicsData.LeftHandPoint.Initialize(character);

            LoadWeaponByIndex();
        }

        public void DeInitialize()
        {
            
        }
        
        public void OnActivate()
        {
            
        }
        
        public void OnDeactivate()
        {
            
        }
        
        public void ChangeActiveWeapon(int index)
        {
            return;
        }
        
        public void AddEquippedFirearmWeapon(ItemData itemData, int orderIndex)
        {
            return;
            //throw new System.NotImplementedException();
        }
        
        public void AddEquippedMeleeWeapon(ItemData itemData)
        {
            var newMeleeWeapon = CreateWeapon(itemData) as MeleeWeapon;
            CurrentActiveMeleeWeapon = newMeleeWeapon;
        }

        private IWeapon CreateWeapon(ItemData itemData)
        {
            var newWeapon = meleeWeaponFactory.Create(itemData);
            characterPhysicsData.RightHandPoint.AddItemInHand(newWeapon, newWeapon.RotationInHands);
            newWeapon.PickUpItem(character);
            newWeapon.gameObject.SetActive(true);

            return newWeapon;
        }

        private void LoadWeaponByIndex()
        {
            var weaponData = itemCollectionData.GetItemByIndex(SWORD_WEAPON_INDEX);
            AddEquippedMeleeWeapon(weaponData);
        }
    }
}