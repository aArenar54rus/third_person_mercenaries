using Arenar.Services.InventoryService;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Arenar.UI
{
    public class ItemInformationPanelControlWithButtons : ItemInformationPanelControl
    {
        [SerializeField]
        private Button buttonPrefab;
        [SerializeField]
        private RectTransform buttonPrefabParent;
        
        private List<Button> activeButtons = new List<Button>();
        private Queue<Button> deactiveButtonQueue = new Queue<Button>();


        public override void ShowInfoPanel(Vector3 itemCellPosition, InventoryItemCellData invItemCellData, bool isEquipped = false)
        {
            base.ShowInfoPanel(itemCellPosition, invItemCellData, isEquipped);
            ClearButtons();
        }

        public void ClearButtons()
        {
            foreach (var activeButton in activeButtons)
            {
                activeButton.onClick.RemoveAllListeners();
                deactiveButtonQueue.Enqueue(activeButton);
            }
            
            activeButtons.Clear();
        }
        
        public void AddButton(string buttonText, Action buttonAction)
        {
            Button newButton;
            newButton = deactiveButtonQueue.Count == 0
                ? Instantiate(buttonPrefab, buttonPrefabParent)
                : deactiveButtonQueue.Dequeue();
            
            activeButtons.Add(newButton);
            newButton.GetComponentInChildren<Text>().text = buttonText;
            newButton.onClick.AddListener(() => buttonAction());
            newButton.gameObject.SetActive(true);
        }
    }
}