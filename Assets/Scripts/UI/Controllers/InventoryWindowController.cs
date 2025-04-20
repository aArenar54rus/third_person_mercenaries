using System.Collections.Generic;
using Arenar.AudioSystem;
using Arenar.Items;
using Arenar.Services.InventoryService;
using Arenar.Services.PlayerInputService;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;


namespace Arenar.Services.UI
{
    public class InventoryWindowController : CanvasWindowController
    {
        private IInventoryService inventoryService;
        private IUiSoundManager _uiSoundManager;

        private InventoryCanvasWindow inventoryWindow;
        private InventoryEquipCanvasLayer inventoryEquipCanvasLayer;
        private InventoryBagCanvasLayer inventoryBagCanvasLayer;
        private InventoryItemDescriptionCanvasLayer inventoryItemDescriptionCanvasLayer;
        private InventoryControlButtonsCanvasLayer inventoryControlButtonsCanvasLayer;

        private List<InventoryCellVisual> activeInventoryBagCells = new();
        private Queue<InventoryCellVisual> deactiveInventoryCells = new();
        
        
        [Inject]
        public InventoryWindowController(IInventoryService inventoryService,
                                         IPlayerInputService playerInputService,
                                         IUiSoundManager uiSoundManager)
            : base(playerInputService)
        {
            this.inventoryService = inventoryService;
            base.playerInputService = playerInputService;
            _uiSoundManager = uiSoundManager;
        }
        
        
        public override void Initialize(ICanvasService canvasService)
        {
            base.Initialize(canvasService);
            
            inventoryWindow = base.canvasService.GetWindow<InventoryCanvasWindow>();
            inventoryEquipCanvasLayer = inventoryWindow.GetWindowLayer<InventoryEquipCanvasLayer>();
            inventoryBagCanvasLayer = inventoryWindow.GetWindowLayer<InventoryBagCanvasLayer>();
            inventoryItemDescriptionCanvasLayer = inventoryWindow.GetWindowLayer<InventoryItemDescriptionCanvasLayer>();
            inventoryControlButtonsCanvasLayer = inventoryWindow.GetWindowLayer<InventoryControlButtonsCanvasLayer>();
            
            inventoryControlButtonsCanvasLayer.BackButton.onClick.AddListener(OnReturnToMenuBtnClick);

            InitializeInventoryBag();
            InitializeInventoryEquip();
            
            UpdateInventoryMassVisual();
            
            inventoryWindow.OnShowEnd.AddListener(OnWindowShowEnd_SelectElements);
            inventoryWindow.OnHideBegin.AddListener(OnWindowHideBegin_DeselectElements);
        }

        protected override void OnWindowShowEnd_SelectElements()
        {
            if (activeInventoryBagCells.Count > 0)
                activeInventoryBagCells[0].Select();
            else
                inventoryControlButtonsCanvasLayer.BackButton.Select();
            
            inventoryControlButtonsCanvasLayer.MoneyWallet.UpdateText(inventoryService.CurrencyMoney);
            
            if (playerInputService.InputActionCollection is PlayerInput playerInput)
                playerInput.UI.Decline.performed += OnInputAction_Decline;
        }

        protected override void OnWindowHideBegin_DeselectElements()
        {
            inventoryItemDescriptionCanvasLayer.MainItemInformationPanelControl.HideInfoPanel();
            inventoryItemDescriptionCanvasLayer.SecondItemInformationPanelControl.HideInfoPanel();
            
            if (playerInputService.InputActionCollection is PlayerInput playerInput)
                playerInput.UI.Decline.performed -= OnInputAction_Decline;
        }

        private void OnInputAction_Decline(InputAction.CallbackContext context)
        {
            if (inventoryItemDescriptionCanvasLayer.MainItemInformationPanelControl.enabled)
            {
                inventoryItemDescriptionCanvasLayer.MainItemInformationPanelControl.HideInfoPanel();
                inventoryItemDescriptionCanvasLayer.SecondItemInformationPanelControl.HideInfoPanel();
                return;
            }
            
            OnReturnToMenuBtnClick();
        }

        private void InitializeInventoryBag()
        {
            var bagItems = inventoryService.GetAllBagItems();
            for (int i = 0; i <  bagItems.Length; i++)
                AddCellBagVisual(i, bagItems[i]);
            
            inventoryService.OnUpdateInventoryCells += OnUpdateInventoryCells;
        }

        private void AddCellBagVisual(int cellIndex, InventoryItemCellData inventoryCellData)
        {
            if (inventoryCellData.itemData == null)
                return;
            
            InventoryCellVisual newBagCell;
            if (deactiveInventoryCells.Count > 0)
            {
                newBagCell = deactiveInventoryCells.Dequeue();
            }
            else
            {
                newBagCell = GameObject.Instantiate(inventoryBagCanvasLayer.InventoryBagCellPrefab,
                    inventoryBagCanvasLayer.InventoryBagCellParent).GetComponent<InventoryCellVisual>();
            }

            newBagCell.gameObject.SetActive(true);
            
            newBagCell.Initialize(cellIndex, inventoryCellData);
            
            newBagCell.onCellSelected += OnCellSelected_InventoryCellBag;
            newBagCell.onCellDeselected += OnCellDeselected_InventoryCellBag;
            newBagCell.onCellClicked += OnCellClicked_InventoryCellBag;
            
            activeInventoryBagCells.Add(newBagCell);
        }
        
        private void RemoveBagCellVisual(int cellIndex)
        {
            if (inventoryService.GetInventoryItemDataByCellIndex(cellIndex) == null)
                return;
            
            if (!TryGetVisualByCellIndex(cellIndex, out InventoryCellVisual neededCellVisual))
                return;
            
            deactiveInventoryCells.Enqueue(neededCellVisual);
            activeInventoryBagCells.Remove(neededCellVisual);
            
            neededCellVisual.onCellSelected -= OnCellSelected_InventoryCellBag;
            neededCellVisual.onCellDeselected -= OnCellDeselected_InventoryCellBag;
            neededCellVisual.onCellClicked -= OnCellClicked_InventoryCellBag;
            
            neededCellVisual.gameObject.SetActive(false);
        }

        private void InitializeInventoryEquip()
        {
            /*
             foreach (var clothItemCell in _inventoryEquipCanvasLayer.ClothItemCells)
            {
                clothItemCell.Value.Initialize(0);
                InventoryItemCellData invItemCellData = inventoryService.GetEquippedCloth(clothItemCell.Key);
                if (invItemCellData == null || invItemCellData.itemInventoryData == null)
                    clothItemCell.Value.SetEmpty();
                else
                    clothItemCell.Value.SetItem(invItemCellData);
            }
            */
            
            InventoryItemCellData[] weaponInvItemCellData = inventoryService.GetEquippedFirearmWeapons();
            for (int i = 0; i < inventoryEquipCanvasLayer.WeaponCells.Length; i++)
            {
                if (weaponInvItemCellData[i] == null || weaponInvItemCellData[i].itemData == null)
                    inventoryEquipCanvasLayer.WeaponCells[i].SetEmpty();
                else
                    inventoryEquipCanvasLayer.WeaponCells[i].Initialize(i, weaponInvItemCellData[i]);
            }
        }

        private void UpdateInventoryMassVisual()
        {
            var massSlider = inventoryBagCanvasLayer.MassSlider;
            massSlider.maxValue = inventoryService.InventoryMassMax;
            massSlider.value = inventoryService.InventoryMass;
        }

        private void OnUpdateInventoryCells(List<int> cellsIndexes)
        {
            foreach (var cellIndex in cellsIndexes)
            {
                var inventoryCellData = inventoryService.GetInventoryItemDataByCellIndex(cellIndex);

                if (inventoryCellData == null)
                    continue;
                
                if (inventoryCellData.ElementsCount <= 0)
                {
                    RemoveBagCellVisual(cellIndex);
                }
                else
                {
                    bool isFound = false;
                    foreach (var activeVisual in activeInventoryBagCells)
                    {
                        if (activeVisual.CellIndex != cellIndex)
                            continue;
                        
                        activeVisual.Initialize(cellIndex, inventoryCellData);
                        isFound = true;
                    }

                    if (!isFound)
                    {
                        AddCellBagVisual(cellIndex, inventoryCellData);
                    }
                }
            }

            UpdateInventoryMassVisual();
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
            InventoryItemCellData inventoryItemCellData = inventoryService.GetInventoryItemDataByCellIndex(cellIndex);
            if (inventoryItemCellData.itemData == null)
            {
                inventoryItemDescriptionCanvasLayer.MainItemInformationPanelControl.HideInfoPanel();
                return;
            }

            bool isFindWeaponForCheck = false;
            if (inventoryItemCellData.itemData is FirearmWeaponItemData weaponData)
            {
                var equippedWeapons = inventoryService.GetEquippedFirearmWeapons();
                
                for (int i = 0; i < equippedWeapons.Length; i++)
                {
                    if (equippedWeapons[i].itemData != null
                        && equippedWeapons[i].itemData is FirearmWeaponItemData equippedWeaponData
                        && weaponData.WeaponType == equippedWeaponData.WeaponType)
                    {
                        inventoryItemDescriptionCanvasLayer.SecondItemInformationPanelControl
                            .ShowInfoPanel(inventoryEquipCanvasLayer.WeaponCells[i].transform.position,
                                equippedWeapons[i],
                                true);
                        isFindWeaponForCheck = true;
                        break;
                    }
                }
            }

            if (!isFindWeaponForCheck)
                inventoryItemDescriptionCanvasLayer.SecondItemInformationPanelControl.HideInfoPanel();
        }

        private void OnCellDeselected_InventoryCellBag(int cellIndex)
        {
            // inventoryItemDescriptionCanvasLayer.MainItemInformationPanelControl.HideInfoPanel();
            // inventoryItemDescriptionCanvasLayer.SecondItemInformationPanelControl.HideInfoPanel();
        }

        private void OnCellClicked_InventoryCellBag(int cellIndex)
        {
            InventoryItemCellData inventoryItemCellData = inventoryService.GetInventoryItemDataByCellIndex(cellIndex);
            if (inventoryItemCellData.itemData == null)
                return;

            if (!TryGetVisualByCellIndex(cellIndex, out InventoryCellVisual cellVisual))
                return;
            
            inventoryItemDescriptionCanvasLayer.MainItemInformationPanelControl
                .ShowInfoPanel(cellVisual.transform.position, inventoryItemCellData, false);

            switch (inventoryItemCellData.itemData.ItemType)
            {
                case ItemType.MeleeWeapon:
                    inventoryItemDescriptionCanvasLayer.MainItemInformationPanelControl.AddButton("Equip", EquipMeleeWeaponButton);
                    break;
                
                case ItemType.Money:
                    inventoryItemDescriptionCanvasLayer.MainItemInformationPanelControl.AddButton("Use", UseMoneyPackageButton);
                    break;
            }
            
            
            void EquipMeleeWeaponButton()
            {
                inventoryService.EquipMeleeWeaponFromBag(cellIndex);
                inventoryService.RemoveItemFromCell(cellIndex, 1, out InventoryItemCellData _);
                
                RemoveBagCellVisual(cellIndex);
                
                inventoryItemDescriptionCanvasLayer.MainItemInformationPanelControl.HideInfoPanel();
                inventoryItemDescriptionCanvasLayer.SecondItemInformationPanelControl.HideInfoPanel();
            }
            
            void UseMoneyPackageButton()
            {
                if (inventoryItemCellData.itemData is MaterialItemData materialItemData)
                    inventoryService.CurrencyMoney += materialItemData.MaterialMight;
                
                inventoryService.RemoveItemFromCell(cellIndex, 1, out InventoryItemCellData _);
                
                inventoryControlButtonsCanvasLayer.MoneyWallet.UpdateText(inventoryService.CurrencyMoney);
            }
        }

        private bool TryGetVisualByCellIndex(int cellIndex, out InventoryCellVisual inventoryItemCellData)
        {
            inventoryItemCellData = null;
            foreach (var activeVisual in activeInventoryBagCells)
            {
                if (activeVisual.CellIndex != cellIndex)
                    continue;

                inventoryItemCellData = activeVisual;
                break;
            }
            
            return inventoryItemCellData != null;
        }
    }
}