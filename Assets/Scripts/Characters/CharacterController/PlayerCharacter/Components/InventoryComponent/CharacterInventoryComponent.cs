using UnityEngine;
using Arenar.Items;
using Arenar.Services.InventoryService;
using Zenject;


namespace Arenar.Character
{
	public class CharacterInventoryComponent : IInventoryComponent
	{
		private ICharacterEntity character;
		private ICharacterAnimationComponent<CharacterAnimationComponent.Animation, CharacterAnimationComponent.AnimationValue> characterAnimationComponent;
		
		private FirearmWeaponFactory firearmWeaponFactory;
		private CharacterPhysicsDataStorage characterPhysicsData;
		
		private IInventoryService inventoryService;
		
		
		public FirearmWeapon CurrentActiveWeapon { get; protected set; }
		public FirearmWeapon[] EquippedFirearmWeapons { get; protected set; }


		[Inject]
		public void Construct(ICharacterEntity character,
							FirearmWeaponFactory firearmWeaponFactory,
							IInventoryService inventoryService,
							ICharacterDataStorage<CharacterPhysicsDataStorage> characterPhysicsDataStorage)
		{
			this.character = character;
			this.firearmWeaponFactory = firearmWeaponFactory;
			this.inventoryService = inventoryService;
			this.characterPhysicsData = characterPhysicsDataStorage.Data;
		}
		
		public void Initialize()
		{
			characterPhysicsData.RightHandPoint.Initialize(character);
			characterPhysicsData.LeftHandPoint.Initialize(character);
			
			if (character.TryGetCharacterComponent<ICharacterAnimationComponent>(out ICharacterAnimationComponent animationComponent))
			{
				if (animationComponent is CharacterAnimationComponent neededAnimationComponent)
					characterAnimationComponent = neededAnimationComponent;
			}
			
			LoadClothes();
			
			LoadWeaponFromInventory();
			ChangeActiveWeapon(0);
		}
		
		public void DeInitialize()
		{
			foreach (var equippedWeapon in EquippedFirearmWeapons)
				GameObject.Destroy(equippedWeapon.gameObject);
		}
		
		public void OnActivate()
		{
			if (EquippedFirearmWeapons.Length > 0)
			{
				ChangeActiveWeapon(0);
			}
		}

		public void OnDeactivate()
		{
			if (CurrentActiveWeapon != null)
			{
				CurrentActiveWeapon.gameObject.SetActive(false);
				CurrentActiveWeapon = null;
			}
		}
		
		public void ChangeActiveWeapon(int index)
		{
			if (CurrentActiveWeapon != null)
			{
				CurrentActiveWeapon.gameObject.SetActive(false);
				CurrentActiveWeapon = null;
			}

			if (index < 0 || EquippedFirearmWeapons.Length == 0)
			{
				return;
			}

			if (index >= EquippedFirearmWeapons.Length)
			{
				index = EquippedFirearmWeapons.Length - 1;
			}
			
			CurrentActiveWeapon = EquippedFirearmWeapons[index];
			
			switch (CurrentActiveWeapon.FirearmWeaponClass)
			{
				default: 
					Debug.LogError($"Unknown weapon type class {CurrentActiveWeapon.FirearmWeaponClass}");
					break;
				
				case FirearmWeaponClass.None:
					characterAnimationComponent.SetAnimationValue(CharacterAnimationComponent.AnimationValue.PistolHands, 0);
					characterAnimationComponent.SetAnimationValue(CharacterAnimationComponent.AnimationValue.ShotgunHands, 0);
					characterAnimationComponent.SetAnimationValue(CharacterAnimationComponent.AnimationValue.RifleHands, 0);
					break;
				
				case FirearmWeaponClass.Pistol:
					characterAnimationComponent.SetAnimationValue(CharacterAnimationComponent.AnimationValue.PistolHands, 1);
					characterAnimationComponent.SetAnimationValue(CharacterAnimationComponent.AnimationValue.ShotgunHands, 0);
					characterAnimationComponent.SetAnimationValue(CharacterAnimationComponent.AnimationValue.RifleHands, 0);
					break;
				
				case FirearmWeaponClass.Shotgun:
					characterAnimationComponent.SetAnimationValue(CharacterAnimationComponent.AnimationValue.PistolHands, 0);
					characterAnimationComponent.SetAnimationValue(CharacterAnimationComponent.AnimationValue.ShotgunHands, 1);
					characterAnimationComponent.SetAnimationValue(CharacterAnimationComponent.AnimationValue.RifleHands, 0);
					break;
				
				case FirearmWeaponClass.Rifle:
					characterAnimationComponent.SetAnimationValue(CharacterAnimationComponent.AnimationValue.PistolHands, 0);
					characterAnimationComponent.SetAnimationValue(CharacterAnimationComponent.AnimationValue.ShotgunHands, 0);
					characterAnimationComponent.SetAnimationValue(CharacterAnimationComponent.AnimationValue.RifleHands, 1);
					break;
			}
			
			CurrentActiveWeapon.gameObject.SetActive(true);
		}
		
		public void AddEquippedFirearmWeapon(ItemInventoryData itemInventoryData, int orderIndex)
		{
			var newWeapon = firearmWeaponFactory.Create(itemInventoryData);
			characterPhysicsData.RightHandPoint.AddItemInHand(newWeapon, newWeapon.RotationInHands);
			newWeapon.PickUpItem(character);
			newWeapon.gameObject.SetActive(false);
				
			if (EquippedFirearmWeapons[orderIndex] != null)
				GameObject.Destroy(EquippedFirearmWeapons[orderIndex].gameObject);
			EquippedFirearmWeapons[orderIndex] = newWeapon;
		}

		private void LoadWeaponFromInventory()
		{
			InventoryItemCellData[] equippedWeaponDatas = inventoryService.GetEquippedWeapons();
			EquippedFirearmWeapons = new FirearmWeapon[equippedWeaponDatas.Length];

			for (int i = 0; i < equippedWeaponDatas.Length; i++)
			{
				var equippedWeaponInventoryData = equippedWeaponDatas[i];
				if (equippedWeaponInventoryData == null
					|| equippedWeaponInventoryData.itemInventoryData == null)
					continue;

				AddEquippedFirearmWeapon(equippedWeaponInventoryData.itemInventoryData, i);
			}
		}
		
		private void LoadClothes()
		{
			// TODO; wardrobe system
		}
	}
}