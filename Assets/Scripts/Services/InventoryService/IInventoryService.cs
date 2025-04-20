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
		
		int CurrencyMoney { get; set; }


		InventoryItemCellData GetInventoryItemDataByCellIndex(int cellIndex);

		InventoryItemCellData[] GetAllBagItems();

		bool TryAddItemsInBag(ItemData itemInventoryData,
						int count,
						out InventoryItemCellData restOfItemsCell);
        
		bool TryAddItemInCurrentCell(int cellIndex, ItemData itemInventoryData, int count, out InventoryItemCellData restOfItemsCell);

		void RemoveItemFromCell(int cellIndex, int count, out InventoryItemCellData restOfItemsCell);

		bool IsEnoughItems(ItemData itemInventoryData, int neededCount);
        
		bool IsEnoughItems(int itemIndex, int neededCount);

		bool TryRemoveItems(ItemData itemInventoryData, int neededCount, out InventoryItemCellData restOfItemsCell);
        
		bool TryRemoveItems(int itemIndex, int neededCount, out InventoryItemCellData restOfItemsCell);

		InventoryItemCellData GetEquippedMeleeWeapon();
		
		InventoryItemCellData[] GetEquippedFirearmWeapons();

		InventoryItemCellData GetEquippedCloth(ItemClothType itemClothType);

		void EquipMeleeWeaponFromBag(int bagItemIndex);
	}
}
