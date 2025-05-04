using System;
using System.Collections.Generic;
using Arenar.PreferenceSystem;
using UnityEngine;


namespace Arenar.Services.InventoryService
{
    public class InventoryService : IInventoryService
    {
        public event Action<List<int>> OnUpdateInventoryCells;
        public event Action<ItemClothType> OnUpdateEquippedClothItemCell;
        public event Action OnUpdateEquippedWeaponItem;
        
        
        private InventoryOptionsSOData.Parameters parameters;
        private ItemCollectionData itemCollectionData;

        private IPreferenceManager preferenceManager;
        private InventorySaveData inventorySaveData;
        
        private InventoryCellData equippedMeleeWeapon;
        private InventoryCellData[] equippedFirearmWeapons;
        private Dictionary<ItemClothType, InventoryCellData> equippedClothItems;
        private Dictionary<int, InventoryCellData> inventoryCells;
        
        private float currentInventoryMass = 0.0f;


        public bool IsMassOverbalance =>
            currentInventoryMass > InventoryMassMax;

        public int InventoryCellsCount =>
            parameters.DefaultInventoryCellsCount;

        public float InventoryMass =>
            currentInventoryMass;

        public int InventoryMassMax =>
            parameters.DefaultMassMax;


        public InventoryService(InventoryOptionsSOData.Parameters parameters,
                                ItemCollectionData itemCollectionData,
                                IPreferenceManager preferenceManager)
        {
            this.parameters = parameters;
            this.preferenceManager = preferenceManager;
            this.itemCollectionData = itemCollectionData;
            
            Initialize();
        }
        
        
        public InventoryCellData GetInventoryItemDataByCellIndex(int cellIndex)
        {
            throw new NotImplementedException();
        }

        public List<InventoryCellData> GetAllBagItems()
        {
            List<InventoryCellData> allItems = new List<InventoryCellData>(inventoryCells.Count);
            int i = 0;
            
            foreach (InventoryCellData cell in inventoryCells.Values)
            {
                if (cell.itemData == null)
                    continue;
                allItems[i] = cell;
                i++;
            }
            
            return allItems;
        }
        
        public InventoryCellData GetEquippedMeleeWeapon()
        {
            return equippedMeleeWeapon;
        }
        
        public InventoryCellData[] GetEquippedFirearmWeapons()
        {
            return equippedFirearmWeapons;
        }

        public InventoryCellData GetEquippedCloth(ItemClothType itemClothType)
        {
            if (equippedClothItems.ContainsKey(itemClothType))
            {
                Debug.LogError($"Unknown cloth type {itemClothType}!");
                return null;
            }
            
            return equippedClothItems[itemClothType];
        }
        
        public void EquipMeleeWeaponFromBag(int bagItemIndex)
        {
            if (inventoryCells[bagItemIndex].itemData == null)
            {
                Debug.LogError($"Bag is empty. Index: {bagItemIndex}!");
                return;
            }

            (equippedMeleeWeapon, inventoryCells[bagItemIndex]) = (inventoryCells[bagItemIndex], equippedMeleeWeapon);
            
            CalculateMass();
            SaveData();
            
            List<int> indexes = new List<int>();
            indexes.Add(bagItemIndex);
            OnUpdateInventoryCells?.Invoke(indexes);
            OnUpdateEquippedWeaponItem?.Invoke();
        }
        
        public bool TryAddItemsInBag(ItemData itemInventoryData, int count, out InventoryCellData restOfCell)
        {
            // first, check mass
            if (itemInventoryData.ItemMass * count > InventoryMassMax - InventoryMass)
            {
                restOfCell = new InventoryCellData(itemInventoryData, count);
                return false;
            }

            /*if (itemInventoryData.ItemType == ItemType.FirearmWeapon)
            {
                for (int i = 0; i < _inventorySaveData.EquippedFirearmWeaponCells.Length; i++)
                {
                    _inventorySaveData.EquippedFirearmWeaponCells[i] ??= new InventoryItemCellData();
                    if (_inventorySaveData.EquippedFirearmWeaponCells[i].itemData != null)
                        continue;
                    
                    _inventorySaveData.EquippedFirearmWeaponCells[i].itemData = itemInventoryData;
                    _inventorySaveData.EquippedFirearmWeaponCells[i].ElementsCount = 1;
                    restOfItemsCell = null;
                    CalculateMass();
                    OnUpdateEquippedWeaponItem?.Invoke();
                    return true;
                }
            }*/

            if (!itemInventoryData.CanStack)
                return TryAddInFreeCell(itemInventoryData, count, out restOfCell);

            //check cells with same item
            List<int> updatedCellIndexes = new List<int>();
            for (int i = 0; i <inventoryCells.Count; i++)
            {
                if (inventoryCells[i].itemData == null)
                    continue;
                
                if (inventoryCells[i].itemData.Id != itemInventoryData.Id
                    || inventoryCells[i].StackIsFull)
                    continue;

                inventoryCells[i].ElementsCount += count;
                count = inventoryCells[i].ElementsCount
                    - inventoryCells[i].itemData.StackCountMax;
                
                updatedCellIndexes.Add(i);
                
                if (count > 0)
                    continue;
                
                CalculateMass();
                OnUpdateInventoryCells?.Invoke(new List<int>(updatedCellIndexes));
                
                restOfCell = null;
                return true;
            }
            
            SaveData();
            
            return TryAddInFreeCell(itemInventoryData, count, out restOfCell);
        }

        public bool TryAddItemInCurrentCell(int cellIndex,
                                            ItemData itemInventoryData,
                                            int count,
                                            out InventoryCellData restOfCell)
        {
            InventoryCellData inventoryCellData = GetInventoryItemDataByCellIndex(cellIndex); 

            if (!itemInventoryData.CanStack)
            {
                if (inventoryCellData.itemData == null)
                {
                    inventoryCellData.itemData = itemInventoryData;
                    inventoryCellData.ElementsCount = 1;
                    restOfCell = null;
                    
                    CalculateMass();
                    SaveData();
                    
                    OnUpdateInventoryCells?.Invoke(new List<int>(cellIndex));
                    return true;
                }

                restOfCell = new InventoryCellData(itemInventoryData, count);
                return false;
            }

            if (inventoryCellData.itemData == null)
            {
                restOfCell = new InventoryCellData(itemInventoryData, count);
                inventoryCells[cellIndex] = restOfCell;
                
                CalculateMass();
                SaveData();
                OnUpdateInventoryCells?.Invoke(new List<int>(1){ cellIndex });
                return true;
            }

            inventoryCellData.ElementsCount += count;
            count = inventoryCellData.ElementsCount - inventoryCellData.itemData.StackCountMax;
            
            CalculateMass();
            SaveData();
            OnUpdateInventoryCells?.Invoke(new List<int>(1){ cellIndex });

            if (count > 0)
            {
                restOfCell = new InventoryCellData(itemInventoryData, count);
                return false;
            }
            
            restOfCell = null;
            return true;
        }

        public void RemoveItemFromCell(int cellIndex, int count, out InventoryCellData restOfCell)
        {
            var dataCell = GetInventoryItemDataByCellIndex(cellIndex);
            if (dataCell.ElementsCount < count)
            {
                restOfCell = null;
                return;
            }
            
            restOfCell = new InventoryCellData(dataCell.itemData, count);
            dataCell.ElementsCount -= count;
            if (dataCell.ElementsCount == 0)
                dataCell.itemData = null;
            
            CalculateMass();
            SaveData();
            
            List<int> usedIndexes = new List<int>();
            usedIndexes.Add(cellIndex);
            
            OnUpdateInventoryCells?.Invoke(usedIndexes);
        }

        public bool IsEnoughItems(ItemData itemInventoryData, int neededCount)
        {
            return IsEnoughItems(itemInventoryData.Id, neededCount);
        }

        public bool IsEnoughItems(int itemIndex, int neededCount)
        {
            int counter = 0;
            
            foreach (var inventoryCell in inventoryCells.Values)
            {
                if (inventoryCell.itemData == null)
                    continue;
                
                if (inventoryCell.itemData.Id != itemIndex)
                    continue;

                counter += inventoryCell.ElementsCount;
                if (counter >= neededCount)
                    return true;
            }

            return false;
        }

        public bool TryRemoveItems(ItemData itemInventoryData, int neededCount, out InventoryCellData restOfCell)
        {
            if (!IsEnoughItems(itemInventoryData.Id, neededCount))
            {
                restOfCell = null;
                return false;
            }

            int counter = 0;
            List<int> changedCellsIndexes = new List<int>();
            
            for (int i = 0; i < inventorySaveData.InventoryBagCells.Length; i++)
            {
                if (inventoryCells[i].itemData.Id != itemInventoryData.Id)
                    continue;

                changedCellsIndexes.Add(i);
                if (inventoryCells[i].ElementsCount <= neededCount)
                {
                    counter += inventoryCells[i].ElementsCount;
                    neededCount -= inventoryCells[i].ElementsCount;
                    inventoryCells[i].itemData = null;
                    inventoryCells[i].ElementsCount = 0;

                    if (neededCount == 0)
                        break;
                }
                else
                {
                    counter += neededCount;
                    inventoryCells[i].ElementsCount -= neededCount;
                    break;
                }
            }

            CalculateMass();
            SaveData();
            OnUpdateInventoryCells?.Invoke(changedCellsIndexes);
            restOfCell = new InventoryCellData(itemInventoryData, counter);
            return true;
        }

        public bool TryRemoveItems(int itemIndex, int neededCount, out InventoryCellData restOfCell)
        {
            if (!IsEnoughItems(itemIndex, neededCount))
            {
                restOfCell = null;
                return false;
            }

            int counter = 0;

            ItemData neededItemInventoryData = null;
            List<int> changedCellsIndexes = new List<int>();
            
            for (int i = 0; i < inventoryCells.Count; i++)
            {
                if (inventoryCells[i].itemData.Id != itemIndex)
                    continue;
                
                changedCellsIndexes.Add(i);
                neededItemInventoryData = inventoryCells[i].itemData;
                if (inventoryCells[i].ElementsCount <= neededCount)
                {
                    counter += inventoryCells[i].ElementsCount;
                    neededCount -= inventoryCells[i].ElementsCount;
                    inventoryCells[i].itemData = null;
                    inventoryCells[i].ElementsCount = 0;

                    if (neededCount == 0)
                        break;
                }
                else
                {
                    counter += neededCount;
                    inventoryCells[i].ElementsCount -= neededCount;
                    break;
                }
            }

            CalculateMass();
            SaveData();
            OnUpdateInventoryCells?.Invoke(changedCellsIndexes);
            restOfCell = new InventoryCellData(neededItemInventoryData, counter);
            return true;
        }

        private void Initialize()
        {
            InitializeInventory();
            
            /*_inventoryItemDataCells = new Dictionary<int, InventoryItemCellData>(InventoryCellsCount);
            for (int i = 0; i < InventoryCellsCount; i++)
                _inventoryItemDataCells.Add(i, new InventoryItemCellData(null, 0));

            _equippedClothItems = new Dictionary<ItemClothType, InventoryItemCellData>();
            _equippedClothItems.Add(ItemClothType.Head, new InventoryItemCellData(null, 0));
            _equippedClothItems.Add(ItemClothType.Body, new InventoryItemCellData(null, 0));
            _equippedClothItems.Add(ItemClothType.Hands, new InventoryItemCellData(null, 0));
            _equippedClothItems.Add(ItemClothType.Foots, new InventoryItemCellData(null, 0));#1#*/
            
            CalculateMass();
        }

        private bool TryAddInFreeCell(ItemData itemInventoryData, int count, out InventoryCellData restOfCell)
        {
            for(int i = 0; i < inventorySaveData.InventoryBagCells.Length; i++)
            {
                if (inventoryCells[i].itemData != null)
                    continue;
                    
                inventoryCells[i].itemData = itemInventoryData;
                inventoryCells[i].ElementsCount = count;
                CalculateMass();
                SaveData();
                restOfCell = null;
                    
                OnUpdateInventoryCells?.Invoke(new List<int>(1){ i });

                return true;
            }

            restOfCell = new InventoryCellData(itemInventoryData, count);
            return false;
        }

        private void CalculateMass()
        {
            currentInventoryMass = 0.0f;
            
            if (parameters.IsMathMass)
            {

                foreach (var inventoryItemData in inventoryCells.Values)
                {
                    if (inventoryItemData == null || inventoryItemData.itemData == null)
                        continue;

                    currentInventoryMass += inventoryItemData.ElementsCount * inventoryItemData.itemData.ItemMass;
                }
            }

            if (parameters.IsMathMassEquipped)
            {
                foreach (var equippedClothItem in equippedClothItems.Values)
                {
                    if (equippedClothItem == null || equippedClothItem.itemData == null)
                        continue;

                    currentInventoryMass += equippedClothItem.itemData.ItemMass;
                }

                foreach (var equippedWeapon in equippedFirearmWeapons)
                {
                    if (equippedWeapon == null || equippedWeapon.itemData == null)
                        continue;

                    currentInventoryMass += equippedWeapon.itemData.ItemMass;
                }
                
                if (equippedMeleeWeapon != null && equippedMeleeWeapon.itemData != null)
                    currentInventoryMass += equippedMeleeWeapon.itemData.ItemMass;
            }
        }

        private void InitializeInventory()
        {
            inventorySaveData = preferenceManager.LoadValue<InventorySaveData>();

            LoadDataFromInitializeFile();

            if (inventorySaveData.EquippedMeleeWeaponsCell.ItemId >= 0)
            {
                ItemData meleeWeaponData = itemCollectionData.GetItemByIndex(
                    inventorySaveData.EquippedMeleeWeaponsCell.ItemId
                );

                equippedMeleeWeapon = new InventoryCellData(meleeWeaponData, inventorySaveData.EquippedMeleeWeaponsCell.ItemCount);
            }
            else
            {
                equippedMeleeWeapon = new InventoryCellData(null, 0);
            }

            equippedFirearmWeapons = new InventoryCellData[parameters.EquippedWeaponsCount];
            for (int i = 0; i < parameters.EquippedWeaponsCount; i++)
            {
                if (inventorySaveData.EquippedFirearmWeaponCells.Length <= i
                    || inventorySaveData.EquippedFirearmWeaponCells[i].ItemId < 0)
                {
                    equippedFirearmWeapons[i] = new InventoryCellData(null, 0);
                    continue;
                }

                ItemData weaponData = itemCollectionData.GetItemByIndex(
                    inventorySaveData.EquippedMeleeWeaponsCell.ItemId
                );

                equippedFirearmWeapons[i] = new InventoryCellData(weaponData, 1);
            }
        }
        
        private void LoadDataFromInitializeFile()
        {
            if (inventorySaveData.IsAddedContentEarly)
                return;
            
            // load melee weapon
            var meleeWeapon = parameters.ConstantMeleeWeaponCellParameters;
            if (meleeWeapon != null)
            {
                if (meleeWeapon.ConstantWeaponId < 0)
                {
                    inventorySaveData.EquippedMeleeWeaponsCell = CreateSaveData(null);
                }
                else
                {
                    ItemData itemData = itemCollectionData.GetItemByIndex(meleeWeapon.ConstantWeaponId);
                    inventorySaveData.EquippedMeleeWeaponsCell = CreateSaveData(itemData.ItemType != ItemType.MeleeWeapon ? null : itemData);
                }
            }
                
            // load firearm weapon data
            foreach (var firearmWeaponParameters in parameters.ConstantWeaponCellParametersArray)
            {
                if (firearmWeaponParameters.ConstantWeaponId < 0)
                {
                    inventorySaveData.EquippedFirearmWeaponCells[firearmWeaponParameters.WeaponCellIndex] = CreateSaveData(null);
                    continue;
                }
                    
                ItemData itemData = itemCollectionData.GetItemByIndex(firearmWeaponParameters.ConstantWeaponId);
                inventorySaveData.EquippedFirearmWeaponCells[firearmWeaponParameters.WeaponCellIndex] =
                    CreateSaveData(itemData.ItemType != ItemType.FirearmWeapon ? null : itemData);
            }

            foreach (var bagCellData in parameters.BagCellParametersArray)
            {
                
            }

            inventorySaveData.IsAddedContentEarly = true;
            preferenceManager.SaveValue(inventorySaveData);
        }

        private InventoryCellSaveData CreateSaveData(ItemData itemData = null)
        {
            InventoryCellData cellData = new InventoryCellData();
            cellData.itemData = itemData;
            cellData.ElementsCount = itemData != null ? 1 : 0;
            cellData.itemLevel = 1;

            InventoryCellSaveData saveData = new InventoryCellSaveData();
            saveData.SetItem(equippedMeleeWeapon);
            return saveData;
        }
        
        private void SaveData()
        {
            // melee weapon
            inventorySaveData.EquippedMeleeWeaponsCell.SetItem(equippedMeleeWeapon);

            // firearm weapon
            for (int i = 0; i < equippedFirearmWeapons.Length; i++)
            {
                inventorySaveData.EquippedFirearmWeaponCells[i]
                    .SetItem(equippedFirearmWeapons[i]);
            }
            
            
            
            preferenceManager.SaveValue(inventorySaveData);
        }
    }
}