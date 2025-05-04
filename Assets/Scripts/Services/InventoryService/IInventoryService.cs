using System;
using System.Collections.Generic;


namespace Arenar.Services.InventoryService
{
	public interface IInventoryService
	{
		event Action<List<int>> OnUpdateInventoryCells;
		event Action<ItemClothType> OnUpdateEquippedClothItemCell;
        
        
		bool IsMassOverbalance { get; }

		int InventoryCellsCount { get; }
        
		float InventoryMass { get; }
        
		int InventoryMassMax { get; }
		

		InventoryCellData GetInventoryItemDataByCellIndex(int cellIndex);

		List<InventoryCellData> GetAllBagItems();

		bool TryAddItemsInBag(ItemData itemInventoryData,
						int count,
						out InventoryCellData restOfCell);
        
		bool TryAddItemInCurrentCell(int cellIndex, ItemData itemInventoryData, int count, out InventoryCellData restOfCell);

		void RemoveItemFromCell(int cellIndex, int count, out InventoryCellData restOfCell);

		bool IsEnoughItems(ItemData itemInventoryData, int neededCount);
        
		bool IsEnoughItems(int itemIndex, int neededCount);

		bool TryRemoveItems(ItemData itemInventoryData, int neededCount, out InventoryCellData restOfCell);
        
		bool TryRemoveItems(int itemIndex, int neededCount, out InventoryCellData restOfCell);

		InventoryCellData GetEquippedMeleeWeapon();
		
		InventoryCellData[] GetEquippedFirearmWeapons();

		InventoryCellData GetEquippedCloth(ItemClothType itemClothType);

		void EquipMeleeWeaponFromBag(int bagItemIndex);
	}
}
