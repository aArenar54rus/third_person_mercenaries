using System.Collections.Generic;
using Arenar.Services.InventoryService;
using Zenject;


namespace Arenar.Services.UI
{
    public class InventoryWindowController : CanvasWindowController
    {
        private IInventoryService _inventoryService;

        private InventoryCanvasWindow _inventoryWindow;
        private MainMenuWindow _mainMenuWindow;

        private InventoryEquipCanvasLayer _inventoryEquipCanvasLayer;
        private InventoryBagCanvasLayer _inventoryBagCanvasLayer;
        private InventoryItemDescriptionCanvasLayer _inventoryItemDescriptionCanvasLayer;
        private InventoryControlButtonsCanvasLayer _inventoryControlButtonsCanvasLayer;
        
        
        [Inject]
        public void Construct(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }
        
        public override void Initialize(ICanvasService canvasService)
        {
            base.Initialize(canvasService);
            
            _mainMenuWindow = _canvasService.GetWindow<MainMenuWindow>();
            _inventoryWindow = _canvasService.GetWindow<InventoryCanvasWindow>();
            
            _inventoryEquipCanvasLayer = _inventoryWindow
                .GetWindowLayer<InventoryEquipCanvasLayer>();
            
            _inventoryBagCanvasLayer = _inventoryWindow
                .GetWindowLayer<InventoryBagCanvasLayer>();
            
            _inventoryItemDescriptionCanvasLayer = _inventoryWindow
                .GetWindowLayer<InventoryItemDescriptionCanvasLayer>();
            
            _inventoryControlButtonsCanvasLayer = _inventoryWindow
                .GetWindowLayer<InventoryControlButtonsCanvasLayer>();
            
            _inventoryControlButtonsCanvasLayer.BackButton.onClick.AddListener(OnReturnToMenuBtnClick);

            InitializeInventoryBag();
            InitializeInventoryEquip();
            
            UpdateInventoryMassVisual();
        }

        private void InitializeInventoryBag()
        {
            for (int i = 0; i < _inventoryBagCanvasLayer.InventoryCells.Length; i++)
            {
                _inventoryBagCanvasLayer.InventoryCells[i].Initialize(i);
                UpdateInventoryCellData(i);
            }

            _inventoryService.OnUpdateInventoryCells += OnUpdateInventoryCells;
        }

        private void InitializeInventoryEquip()
        {
            foreach (var clothItemCell in _inventoryEquipCanvasLayer.ClothItemCells)
            {
                clothItemCell.Value.Initialize(0);
                InventoryItemData invItemData = _inventoryService.GetEquippedCloth(clothItemCell.Key);
                if (invItemData == null || invItemData.itemData == null)
                    clothItemCell.Value.SetEmpty();
                else
                    clothItemCell.Value.SetItem(invItemData);
            }
            
            InventoryItemData[] weaponInvItemData = _inventoryService.GetEquippedWeapons();
            for (int i = 0; i < _inventoryEquipCanvasLayer.WeaponCells.Length; i++)
            {
                _inventoryEquipCanvasLayer.WeaponCells[i].Initialize(0);
                if (weaponInvItemData[i] == null || weaponInvItemData[i].itemData == null)
                    _inventoryEquipCanvasLayer.WeaponCells[i].SetEmpty();
                else
                    _inventoryEquipCanvasLayer.WeaponCells[i].SetItem(weaponInvItemData[i]);
            }
        }

        private void UpdateInventoryMassVisual()
        {
            var massSlider = _inventoryBagCanvasLayer.MassSlider;
            massSlider.maxValue = _inventoryService.InventoryMassMax;
            massSlider.value = _inventoryService.InventoryMass;
        }

        private void OnUpdateInventoryCells(List<int> cellsIndexes)
        {
            foreach (int cellIndex in cellsIndexes)
                UpdateInventoryCellData(cellIndex);

            UpdateInventoryMassVisual();
        }

        private void UpdateInventoryCellData(int cellIndex)
        {
            var itemData = _inventoryService.GetInventoryItemData(cellIndex);
            if (itemData.itemData == null)
            {
                _inventoryBagCanvasLayer.InventoryCells[cellIndex].SetEmpty();
            }
            else
            {
                _inventoryBagCanvasLayer.InventoryCells[cellIndex].SetItem(itemData);
            }
        }

        private void OnReturnToMenuBtnClick()
        {
            _inventoryWindow.Hide(false);
            _mainMenuWindow.Show(false);
        }
    }
}