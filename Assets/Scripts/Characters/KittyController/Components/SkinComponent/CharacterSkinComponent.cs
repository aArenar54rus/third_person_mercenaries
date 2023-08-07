using Arenar.Services.InventoryService;
using UnityEngine;
using Zenject;


namespace Arenar.Character
{
    public class CharacterSkinComponent : ICharacterSkinComponent
    {
        private IInventoryService inventoryService;
        private CharacterPhysicsDataStorage characterPhysicsData;
        private GameObject weaponObject;
        private bool isWeaponEquipped = false;


        [Inject]
        public void Construct(IInventoryService inventoryService,
            ICharacterDataStorage<CharacterPhysicsDataStorage> characterPhysicsDataStorage)
        {
            this.inventoryService = inventoryService;
            characterPhysicsData = characterPhysicsDataStorage.Data;
        }

        public void Initialize()
        {
            inventoryService.OnUpdateEquippedWeaponItem += OnUpdateEquippedWeaponItem;
        }

        public void DeInitialize()
        {
            inventoryService.OnUpdateEquippedWeaponItem -= OnUpdateEquippedWeaponItem;
        }

        public void OnStart()
        {
            OnUpdateEquippedWeaponItem();
        }

        public void SetSkin()
        {

        }

        public void SetWeapon(ItemData itemData)
        {
            /*if (itemData.ItemType != ItemType.Weapon)
                return;
            
            weaponObject = GameObject.Instantiate(
                Resources.Load<GameObject>("Prefabs/Items/" + itemData.Id),
                characterPhysicsData.RightHandPoint);
            weaponObject.transform.localPosition = Vector3.zero;
            weaponObject.transform.localRotation =
                Quaternion.Euler(weaponObject.GetComponent<FirearmWeapon>().LocalRotation);
            isWeaponEquipped = true;*/
        }

        private void OnUpdateEquippedWeaponItem()
        {
            /*InventoryItemData weaponInventoryItemData = inventoryService.GetEquippedWeapon();

            if (isWeaponEquipped)
            {
                if (weaponInventoryItemData == null
                    || weaponInventoryItemData.itemData == null)
                {
                    if (weaponObject != null)
                    {
                        GameObject.Destroy(weaponObject);
                        weaponObject = null;
                    }

                    isWeaponEquipped = false;
                }

                return;
            }

            SetWeapon(weaponInventoryItemData.itemData);*/
        }
    }
}