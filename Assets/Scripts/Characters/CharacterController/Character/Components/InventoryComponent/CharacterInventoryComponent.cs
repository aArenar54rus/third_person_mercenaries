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
		private MeleeWeaponFactory meleeWeaponFactory;
		private CharacterPhysicsDataStorage characterPhysicsData;
		
		private IInventoryService inventoryService;
		
		
		public MeleeWeapon CurrentActiveMeleeWeapon { get; protected set; }
		public FirearmWeapon CurrentActiveFirearmWeapon { get; protected set; }
		public FirearmWeapon[] EquippedFirearmWeapons { get; protected set; }


		[Inject]
		public void Construct(ICharacterEntity character,
							FirearmWeaponFactory firearmWeaponFactory,
							MeleeWeaponFactory meleeWeaponFactory,
							IInventoryService inventoryService,
							ICharacterDataStorage<CharacterPhysicsDataStorage> characterPhysicsDataStorage)
		{
			this.character = character;
			this.firearmWeaponFactory = firearmWeaponFactory;
			this.meleeWeaponFactory = meleeWeaponFactory;
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
			
			LoadWeaponFromInventory();
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
			if (CurrentActiveFirearmWeapon != null)
			{
				CurrentActiveFirearmWeapon.gameObject.SetActive(false);
				CurrentActiveFirearmWeapon = null;
			}
		}
		
		public void ChangeActiveWeapon(int index)
		{
			if (CurrentActiveFirearmWeapon != null)
			{
				CurrentActiveFirearmWeapon.gameObject.SetActive(false);
				CurrentActiveFirearmWeapon = null;
			}

			if (index < 0 || EquippedFirearmWeapons.Length == 0)
			{
				return;
			}

			if (index >= EquippedFirearmWeapons.Length)
			{
				index = EquippedFirearmWeapons.Length - 1;
			}
			
			CurrentActiveFirearmWeapon = EquippedFirearmWeapons[index];
			
			switch (CurrentActiveFirearmWeapon.FirearmWeaponClass)
			{
				default: 
					Debug.LogError($"Unknown weapon type class {CurrentActiveFirearmWeapon.FirearmWeaponClass}");
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
			
			CurrentActiveFirearmWeapon.gameObject.SetActive(true);
		}
		
		public void AddEquippedFirearmWeapon(ItemData itemData, int orderIndex)
		{
			var newWeapon = CreateWeapon(itemData) as FirearmWeapon;
				
			if (EquippedFirearmWeapons[orderIndex] != null)
				GameObject.Destroy(EquippedFirearmWeapons[orderIndex].gameObject);
			EquippedFirearmWeapons[orderIndex] = newWeapon;
		}

		public void AddEquippedMeleeWeapon(ItemData itemData)
		{
			var newMeleeWeapon = CreateWeapon(itemData) as MeleeWeapon;
			CurrentActiveMeleeWeapon = newMeleeWeapon;
		}

		private IWeapon CreateWeapon(ItemData itemData)
		{
			var newWeapon = firearmWeaponFactory.Create(itemData);
			characterPhysicsData.RightHandPoint.AddItemInHand(newWeapon, newWeapon.RotationInHands);
			newWeapon.PickUpItem(character);
			newWeapon.gameObject.SetActive(false);

			return newWeapon;
		}

		private void LoadWeaponFromInventory()
		{
			InventoryCellData[] equippedWeaponDatas = inventoryService.GetEquippedFirearmWeapons();
			EquippedFirearmWeapons = new FirearmWeapon[equippedWeaponDatas.Length];

			for (int i = 0; i < equippedWeaponDatas.Length; i++)
			{
				var equippedWeaponInventoryData = equippedWeaponDatas[i];
				if (equippedWeaponInventoryData == null
					|| equippedWeaponInventoryData.itemData == null)
					continue;

				AddEquippedFirearmWeapon(equippedWeaponInventoryData.itemData, i);
			}
		}
	}
}