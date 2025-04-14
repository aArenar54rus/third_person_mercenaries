using System.Collections.Generic;
using Arenar.AudioSystem;
using Arenar.Items;
using Arenar.Services.InventoryService;
using Arenar.Services.PlayerInputService;
using UnityEngine.InputSystem;
using Zenject;


namespace Arenar.Services.UI
{
    public class InventoryWindowController : CanvasWindowController
    {
        private IInventoryService _inventoryService;

        private InventoryCanvasWindow _inventoryWindow;
        private InventoryEquipCanvasLayer _inventoryEquipCanvasLayer;
        private InventoryBagCanvasLayer _inventoryBagCanvasLayer;
        private InventoryItemDescriptionCanvasLayer _inventoryItemDescriptionCanvasLayer;
        private InventoryControlButtonsCanvasLayer _inventoryControlButtonsCanvasLayer;
        
        private IUiSoundManager _uiSoundManager;
        
        
        [Inject]
        public InventoryWindowController(IInventoryService inventoryService,
                                         IPlayerInputService playerInputService,
                                         IUiSoundManager uiSoundManager)
            : base(playerInputService)
        {
            _inventoryService = inventoryService;
            base.playerInputService = playerInputService;
            _uiSoundManager = uiSoundManager;
        }
        
        
        public override void Initialize(ICanvasService canvasService)
        {
            base.Initialize(canvasService);
            
            _inventoryWindow = base.canvasService.GetWindow<InventoryCanvasWindow>();
            _inventoryEquipCanvasLayer = _inventoryWindow.GetWindowLayer<InventoryEquipCanvasLayer>();
            _inventoryBagCanvasLayer = _inventoryWindow.GetWindowLayer<InventoryBagCanvasLayer>();
            _inventoryItemDescriptionCanvasLayer = _inventoryWindow.GetWindowLayer<InventoryItemDescriptionCanvasLayer>();
            _inventoryControlButtonsCanvasLayer = _inventoryWindow.GetWindowLayer<InventoryControlButtonsCanvasLayer>();
            
            _inventoryControlButtonsCanvasLayer.BackButton.onClick.AddListener(OnReturnToMenuBtnClick);

            InitializeInventoryBag();
            InitializeInventoryEquip();
            
            UpdateInventoryMassVisual();
            
            _inventoryWindow.OnShowEnd.AddListener(OnWindowShowEnd_SelectElements);
            _inventoryWindow.OnHideBegin.AddListener(OnWindowHideBegin_DeselectElements);
        }

        protected override void OnWindowShowEnd_SelectElements()
        {
            _inventoryBagCanvasLayer.InventoryCells[0].Select();
            if (playerInputService.InputActionCollection is PlayerInput playerInput)
                playerInput.UI.Decline.performed += OnInputAction_Decline;
        }

        protected override void OnWindowHideBegin_DeselectElements()
        {
            _inventoryItemDescriptionCanvasLayer.MainItemInformationPanelControl.HideInfoPanel();
            _inventoryItemDescriptionCanvasLayer.SecondItemInformationPanelControl.HideInfoPanel();
            
            if (playerInputService.InputActionCollection is PlayerInput playerInput)
                playerInput.UI.Decline.performed -= OnInputAction_Decline;
        }

        private void OnInputAction_Decline(InputAction.CallbackContext context)
        {
            OnReturnToMenuBtnClick();
        }

        private void InitializeInventoryBag()
        {
            for (int i = 0; i < _inventoryBagCanvasLayer.InventoryCells.Length; i++)
            {
                _inventoryBagCanvasLayer.InventoryCells[i].Initialize(i);
                _inventoryBagCanvasLayer.InventoryCells[i].onCellSelected += OnCellSelected_InventoryCellBag;
                _inventoryBagCanvasLayer.InventoryCells[i].onCellDeselected += OnCellDeselected_InventoryCellBag;
                _inventoryBagCanvasLayer.InventoryCells[i].onCellClicked += OnCellClicked_InventoryCellBag;
                UpdateInventoryCellData(i);
            }

            _inventoryService.OnUpdateInventoryCells += OnUpdateInventoryCells;
        }

        private void InitializeInventoryEquip()
        {
            foreach (var clothItemCell in _inventoryEquipCanvasLayer.ClothItemCells)
            {
                clothItemCell.Value.Initialize(0);
                InventoryItemCellData invItemCellData = _inventoryService.GetEquippedCloth(clothItemCell.Key);
                if (invItemCellData == null || invItemCellData.itemInventoryData == null)
                    clothItemCell.Value.SetEmpty();
                else
                    clothItemCell.Value.SetItem(invItemCellData);
            }
            
            InventoryItemCellData[] weaponInvItemCellData = _inventoryService.GetEquippedWeapons();
            for (int i = 0; i < _inventoryEquipCanvasLayer.WeaponCells.Length; i++)
            {
                _inventoryEquipCanvasLayer.WeaponCells[i].Initialize(0);
                if (weaponInvItemCellData[i] == null || weaponInvItemCellData[i].itemInventoryData == null)
                    _inventoryEquipCanvasLayer.WeaponCells[i].SetEmpty();
                else
                    _inventoryEquipCanvasLayer.WeaponCells[i].SetItem(weaponInvItemCellData[i]);
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
            if (itemData.itemInventoryData == null)
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
            _uiSoundManager.PlaySound(UiSoundType.StandartButtonClick);
            canvasService.TransitionController
                .PlayTransition<TransitionCrossFadeCanvasWindowLayerController,
                        InventoryCanvasWindow,
                        MainMenuWindow>
                    (false, false, null);
        }

        private void OnCellSelected_InventoryCellBag(int cellIndex)
        {
            InventoryItemCellData itemData = _inventoryService.GetInventoryItemData(cellIndex);
            if (itemData.itemInventoryData == null)
            {
                _inventoryItemDescriptionCanvasLayer.MainItemInformationPanelControl.HideInfoPanel();
                return;
            }

            _inventoryItemDescriptionCanvasLayer.MainItemInformationPanelControl
                .ShowInfoPanel(_inventoryBagCanvasLayer.InventoryCells[cellIndex].transform.position, itemData, false);

            bool isFindWeaponForCheck = false;
            if (itemData.itemInventoryData is FirearmWeaponInventoryItemData weaponData)
            {
                var equippedWeapons = _inventoryService.GetEquippedWeapons();
                
                for (int i = 0; i < equippedWeapons.Length; i++)
                {
                    if (equippedWeapons[i].itemInventoryData != null
                        && equippedWeapons[i].itemInventoryData is FirearmWeaponInventoryItemData equippedWeaponData
                        && weaponData.WeaponType == equippedWeaponData.WeaponType)
                    {
                        _inventoryItemDescriptionCanvasLayer.SecondItemInformationPanelControl
                            .ShowInfoPanel(_inventoryEquipCanvasLayer.WeaponCells[i].transform.position,
                                equippedWeapons[i],
                                true);
                        isFindWeaponForCheck = true;
                        break;
                    }
                }
            }

            if (!isFindWeaponForCheck)
                _inventoryItemDescriptionCanvasLayer.SecondItemInformationPanelControl.HideInfoPanel();
        }

        private void OnCellDeselected_InventoryCellBag(int cellIndex)
        {
            _inventoryItemDescriptionCanvasLayer.MainItemInformationPanelControl.HideInfoPanel();
            _inventoryItemDescriptionCanvasLayer.SecondItemInformationPanelControl.HideInfoPanel();
        }

        private void OnCellClicked_InventoryCellBag(int cellIndex)
        {
            InventoryItemCellData itemData = _inventoryService.GetInventoryItemData(cellIndex);
            if (itemData.itemInventoryData == null)
                return;
        }
    }
}