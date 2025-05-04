using Arenar.Services.InventoryService;
using Arenar.Services.PlayerInputService;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;


namespace Arenar.Services.UI
{
    public class InventoryDebugWindowController : CanvasWindowController
    {
        private ItemCollectionData itemCollectionData;
        private IInventoryService inventoryService;
        
        private InventoryControlButtonsCanvasLayer inventoryControlButtonsCanvasLayer;
        private InventoryDebugCanvasLayer inventoryDebugCanvasLayer;
        
        private List<string> allItemsInGameNamesList = null;
        private List<string> activeItemsList = null;


        [Inject]
        public InventoryDebugWindowController(ItemCollectionData itemCollectionData, IInventoryService inventoryService, IPlayerInputService playerInputService)
            : base(playerInputService)
        {
            this.itemCollectionData = itemCollectionData;
            this.inventoryService = inventoryService;
        }


        public override void Initialize(ICanvasService canvasService)
        {
            base.Initialize(canvasService);
            
            var inventoryWindow = base.canvasService.GetWindow<InventoryCanvasWindow>();
            inventoryControlButtonsCanvasLayer = inventoryWindow.GetWindowLayer<InventoryControlButtonsCanvasLayer>();
            inventoryDebugCanvasLayer = inventoryWindow.GetWindowLayer<InventoryDebugCanvasLayer>();

            inventoryControlButtonsCanvasLayer.DebugButton.onClick.AddListener(OnOpenDebugButtonHandler);

            FormItemsForRemove();
            inventoryDebugCanvasLayer.RemoveItemCountField.text = "1";
            inventoryDebugCanvasLayer.RemoveItemCountField.onValueChanged.AddListener(ValidateInputRemove);
            inventoryDebugCanvasLayer.RemoveItemButton.onClick.AddListener(RemoveItemsButtonHandler);

            FormItemsForAdd();
            inventoryDebugCanvasLayer.AddItemCountField.text = "1";
            inventoryDebugCanvasLayer.AddItemCountField.onValueChanged.AddListener(ValidateInputAdd);
            inventoryDebugCanvasLayer.AddItemButton.onClick.AddListener(AddItemsButtonHandler);
            
            inventoryDebugCanvasLayer.LogAllItemsInInventoryButton.onClick.AddListener(LogAllItemsButtonHandler);
        }

        protected override void OnWindowShowEnd_SelectElements()
        {
            inventoryDebugCanvasLayer.gameObject.SetActive(false);
        }
        
        protected override void OnWindowHideBegin_DeselectElements()
        {
            inventoryDebugCanvasLayer.gameObject.SetActive(false);
        }
        
        private void OnOpenDebugButtonHandler()
        {
            inventoryDebugCanvasLayer.gameObject.SetActive(true);
        }

        private void FormItemsForRemove()
        {
            InventoryCellData[] bagItems = inventoryService.GetAllBagItems();
            List<InventoryCellData> itemsToRemove = new List<InventoryCellData>(bagItems.Length);
            foreach (InventoryCellData item in bagItems)
            {
                if (item.itemData != null)
                    itemsToRemove.Add(item);
            }
            
            activeItemsList = itemsToRemove.Select(item => item.itemData.NameKey).ToList();
            
            inventoryDebugCanvasLayer.RemoveItemDropdown.ClearOptions();
            inventoryDebugCanvasLayer.RemoveItemDropdown.AddOptions(activeItemsList);
        }
        
        private void FormItemsForAdd()
        {
            if (allItemsInGameNamesList != null)
                return;
            
            List<ItemData> allItemsInGame = new List<ItemData>(itemCollectionData.GameItems.Count);
            foreach (var itemData in itemCollectionData.GameItems.Values)
            {
                allItemsInGame.Add(itemData);
            }

            allItemsInGameNamesList = allItemsInGame.Select(item => item.NameKey).ToList();
            
            inventoryDebugCanvasLayer.AddItemDropdown.ClearOptions();
            inventoryDebugCanvasLayer.AddItemDropdown.AddOptions(allItemsInGameNamesList);
        }
        
        private void RemoveItemsButtonHandler()
        {
            foreach (var inventoryBagItemData in inventoryService.GetAllBagItems())
            {
                if (inventoryBagItemData.itemData == null)
                    continue;

                string itemName = activeItemsList[inventoryDebugCanvasLayer.RemoveItemDropdown.value];
                if (!inventoryBagItemData.itemData.CanStack)
                    inventoryDebugCanvasLayer.RemoveItemCountField.text = "1";

                int count = int.Parse(inventoryDebugCanvasLayer.RemoveItemCountField.text);

                if (inventoryBagItemData.itemData.NameKey.Equals(itemName))
                {
                    inventoryService.TryRemoveItems(inventoryBagItemData.itemData, count, out InventoryCellData _);
                    break;
                }
            }

            inventoryDebugCanvasLayer.RemoveItemCountField.text = "1";
            FormItemsForRemove();
            
            inventoryDebugCanvasLayer.gameObject.SetActive(false);
        }

        private void AddItemsButtonHandler()
        {
            string itemName = activeItemsList[inventoryDebugCanvasLayer.AddItemDropdown.value];
            
            foreach (ItemData itemData in itemCollectionData.GameItems.Values)
            {
                if (itemData.NameKey.Equals(itemName))
                {
                    if (!itemData.CanStack)
                        inventoryDebugCanvasLayer.RemoveItemCountField.text = "1";
                    int count = int.Parse(inventoryDebugCanvasLayer.AddItemCountField.text);
                    inventoryService.TryAddItemsInBag(itemData, count, out InventoryCellData _);
                    break;
                }
            }
            
            inventoryDebugCanvasLayer.AddItemCountField.text = "1";
            inventoryDebugCanvasLayer.gameObject.SetActive(false);
        }
        
        private void LogAllItemsButtonHandler()
        {
            var items = inventoryService.GetAllBagItems();
            
            Debug.LogError("Выводим список всех предметов:");
            foreach (var itemData in items)
            {
                Debug.LogError($"item {itemData.itemData.NameKey}; count = {itemData.ElementsCount}");
                Debug.LogError("===========");
            }
            Debug.LogError("Конец");
        }

        private void ValidateInputRemove(string text)
        {
            ValidateInput(text, inventoryDebugCanvasLayer.RemoveItemCountField);
        }

        private void ValidateInputAdd(string text)
        {
            ValidateInput(text, inventoryDebugCanvasLayer.AddItemCountField);
        }
        
        private void ValidateInput(string text, TMP_InputField inputField)
        {
            string filteredText = "";
            
            foreach (char c in text)
            {
                if (char.IsDigit(c))
                    filteredText += c;
            }

            if (filteredText != text)
            {
                inputField.text = filteredText;
            }
        }
    }
}