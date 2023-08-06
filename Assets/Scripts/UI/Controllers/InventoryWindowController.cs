using System.Collections.Generic;
using Arenar.Services.InventoryService;
using Arenar.Services.UI;
using Zenject;


namespace Arenar.UI
{
    public class InventoryWindowController : CanvasWindowController
    {
        private IInventoryService inventoryService;
        private PlayerCharacterSpawnController playerCharacterSpawnController;


        private InventoryEquipCanvasLayer inventoryEquipCanvasLayer;
        private InventoryBagCanvasLayer inventoryBagCanvasLayer;
        private InventoryItemDescriptionCanvasLayer inventoryItemDescriptionCanvasLayer;        
        
        
        [Inject]
        public void Construct(PlayerCharacterSpawnController playerCharacterSpawnController,
                              IInventoryService inventoryService)
        {
            this.playerCharacterSpawnController = playerCharacterSpawnController;
            this.inventoryService = inventoryService;
        }
        
        public override void Initialize(ICanvasService canvasService)
        {
            base.Initialize(canvasService);

            var inventoryWindow = base.canvasService
                .GetWindow<InventoryCanvasWindow>();
            
            inventoryEquipCanvasLayer = inventoryWindow
                .GetWindowLayer<InventoryEquipCanvasLayer>();
            
            inventoryBagCanvasLayer = inventoryWindow
                .GetWindowLayer<InventoryBagCanvasLayer>();
            
            inventoryItemDescriptionCanvasLayer = inventoryWindow
                .GetWindowLayer<InventoryItemDescriptionCanvasLayer>();

            InitializeInventoryBag();
            InitializeInventoryEquip();
            
            UpdateInventoryMassVisual();
        }

        private void InitializeInventoryBag()
        {
            for (int i = 0; i < inventoryBagCanvasLayer.InventoryCells.Length; i++)
            {
                inventoryBagCanvasLayer.InventoryCells[i].Initialize(i);
                UpdateInventoryCellData(i);
            }

            inventoryService.OnUpdateInventoryCells += OnUpdateInventoryCells;
        }

        private void InitializeInventoryEquip()
        {
            foreach (var clothItemCell in inventoryEquipCanvasLayer.ClothItemCells)
            {
                clothItemCell.Value.Initialize(0);
                InventoryItemData invItemData = inventoryService.GetEquippedCloth(clothItemCell.Key);
                if (invItemData == null || invItemData.itemData == null)
                    clothItemCell.Value.SetEmpty();
                else
                    clothItemCell.Value.SetItem(invItemData);
            }
            
            inventoryEquipCanvasLayer.WeaponCell.Initialize(0);
            InventoryItemData weaponInvItemData = inventoryService.GetEquippedWeapon();
            if (weaponInvItemData == null || weaponInvItemData.itemData == null)
                inventoryEquipCanvasLayer.WeaponCell.SetEmpty();
            else
                inventoryEquipCanvasLayer.WeaponCell.SetItem(weaponInvItemData);
        }

        private void UpdateInventoryMassVisual()
        {
            var massSlider = inventoryBagCanvasLayer.MassSlider;
            massSlider.maxValue = inventoryService.InventoryMassMax;
            massSlider.value = inventoryService.InventoryMass;
        }

        private void OnUpdateInventoryCells(List<int> cellsIndexes)
        {
            foreach (int cellIndex in cellsIndexes)
                UpdateInventoryCellData(cellIndex);

            UpdateInventoryMassVisual();
        }

        private void UpdateInventoryCellData(int cellIndex)
        {
            var itemData = inventoryService.GetInventoryItemData(cellIndex);
            if (itemData.itemData == null)
            {
                inventoryBagCanvasLayer.InventoryCells[cellIndex].SetEmpty();
            }
            else
            {
                inventoryBagCanvasLayer.InventoryCells[cellIndex].SetItem(itemData);
            }
        }
    }
}