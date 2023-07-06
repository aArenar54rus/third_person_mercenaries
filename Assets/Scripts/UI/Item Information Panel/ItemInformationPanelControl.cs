using System.Collections.Generic;
using Arenar.Services.InventoryService;
using UnityEngine;


namespace Arenar
{
    public class ItemInformationPanelControl : MonoBehaviour
    {
        [SerializeField] private RectTransform informationPanel;
        [SerializeField] private RectTransform content;
        [SerializeField] private float maxHeightPanel;

        [Space(10), Header("Sub Panels")]
        [SerializeField] private EquippedSubPanel equippedSubPanel;
        [SerializeField] private SerializableDictionary<ItemType, SubPanel> mainSubPanels;
        [SerializeField] private SubPanel parametersPanel = default;
        
        private List<SubPanel> spawnedSubPanels;


        public void SetVisible(InventoryItemData invItemData, bool isEquipped = false)
        {
            List<SubPanel> newSpawnedSubPanels = new List<SubPanel>();
            if (isEquipped)
            {
                newSpawnedSubPanels.Add(GetSubPanel(invItemData.itemData, (EquippedSubPanel)equippedSubPanel));
            }

            ItemType itemType = invItemData.itemData.ItemType;
            SubPanel mainSubPanel = mainSubPanels[itemType];
            switch (invItemData.itemData.ItemType)
            {
                case ItemType.Material:
                    newSpawnedSubPanels.Add(GetSubPanel(invItemData.itemData, (MaterialItemDescriptionSubPanel)mainSubPanel));
                    break;
                
                case ItemType.Weapon:
                    newSpawnedSubPanels.Add(GetSubPanel(invItemData.itemData, (WeaponItemDescriptionSubPanel)mainSubPanel));
                    break;
                
                case ItemType.Cloth:
                    newSpawnedSubPanels.Add(GetSubPanel(invItemData.itemData, (ClothItemDescriptionSubPanel)mainSubPanel));
                    break;
                
                case ItemType.Quest:
                    newSpawnedSubPanels.Add(GetSubPanel(invItemData.itemData, (QuestItemDescriptionSubPanel)mainSubPanel));
                    break;
                
                default:
                    Debug.LogError($"Unknown type {invItemData.itemData.ItemType} for info panel.");
                    break;
            }

            if (itemType == ItemType.Weapon
                || itemType == ItemType.Cloth)
            {
                newSpawnedSubPanels.Add(GetSubPanel(invItemData.itemData, (ItemParametersSubPanel)mainSubPanel));
            }
            
            if (spawnedSubPanels.Count != 0)
            {
                foreach (var subPanel in spawnedSubPanels)
                {
                    subPanel.gameObject.SetActive(false);
                    Destroy(subPanel.gameObject);
                }
            }
            spawnedSubPanels = newSpawnedSubPanels;
            
            gameObject.SetActive(true);
            SetPanelHeight();
        }

        private SubPanel GetSubPanel<T>(ItemData itemData, T subPanel)
            where T : SubPanel
        {
            SubPanel instance = GetSubPanelInstance<T>();
            if (instance == null)
                instance = GameObject.Instantiate(subPanel, content);
            instance.Initialize(itemData);
            return instance;
        }

        private SubPanel GetSubPanelInstance<T>()
            where T : SubPanel
        {
            if (spawnedSubPanels == null
                || spawnedSubPanels.Count == 0)
                return null;

            foreach (var subPanel in spawnedSubPanels)
            {
                if (subPanel is T)
                {
                    spawnedSubPanels.Remove(subPanel);
                    return subPanel;
                }
            }

            return null;
        }

        private void SetPanelHeight()
        {
            float contentHeight = content.rect.height;
            informationPanel.sizeDelta = new Vector2 (informationPanel.sizeDelta.x, (contentHeight > maxHeightPanel) ? maxHeightPanel : contentHeight);
        }
    }
}