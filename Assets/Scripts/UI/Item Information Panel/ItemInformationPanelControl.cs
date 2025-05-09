using System.Collections.Generic;
using Arenar.Services.InventoryService;
using UnityEngine;


namespace Arenar.UI
{
    public class ItemInformationPanelControl : MonoBehaviour
    {
        [SerializeField]
        private RectTransform informationPanel;
        [SerializeField]
        private RectTransform content;
        [SerializeField]
        private float maxHeightPanel;
        [SerializeField]
        private float addedHeight;

        [Space(10), Header("Sub Panels")]
        [SerializeField]
        private EquippedSubPanel equippedSubPanel;
        [SerializeField]
        private SerializableDictionary<ItemType, SubPanel> mainSubPanels;
        [SerializeField]
        private ItemParametersSubPanel _itemParametersSubPanel;

        private List<SubPanel> _spawnedSubPanels;


        public virtual void ShowInfoPanel(Vector3 itemCellPosition, InventoryCellData invCellData, bool isEquipped = false)
        {
            List<SubPanel> newSpawnedSubPanels = new List<SubPanel>();
            if (isEquipped)
            {
                newSpawnedSubPanels.Add(GetSubPanel(invCellData.itemData, (EquippedSubPanel)equippedSubPanel));
            }

            ItemType itemType = invCellData.itemData.ItemType;
            SubPanel mainSubPanel = mainSubPanels[itemType];

            switch (invCellData.itemData.ItemType)
            {
                case ItemType.Money:
                    newSpawnedSubPanels.Add(GetSubPanel<MoneyItemDescriptionSubPanel>(invCellData.itemData,
                        (MoneyItemDescriptionSubPanel)mainSubPanel));
                    break;
                
                case ItemType.Material:
                    newSpawnedSubPanels.Add(GetSubPanel<MaterialItemDescriptionSubPanel>(invCellData.itemData,
                        (MaterialItemDescriptionSubPanel)mainSubPanel));
                    break;
                
                case ItemType.MeleeWeapon:
                case ItemType.FirearmWeapon:
                    newSpawnedSubPanels.Add(GetSubPanel<WeaponItemDescriptionSubPanel>(invCellData.itemData,
                        (WeaponItemDescriptionSubPanel)mainSubPanel));
                    break;
                
                case ItemType.Cloth:
                    newSpawnedSubPanels.Add(GetSubPanel<ClothItemDescriptionSubPanel>(invCellData.itemData,
                        (ClothItemDescriptionSubPanel)mainSubPanel));
                    break;
                
                case ItemType.Quest:
                    newSpawnedSubPanels.Add(GetSubPanel<QuestItemDescriptionSubPanel>(invCellData.itemData,
                        (QuestItemDescriptionSubPanel)mainSubPanel));
                    break;
                
                default:
                    Debug.LogError($"Unknown type {invCellData.itemData.ItemType} for info panel.");
                    break;
            }

            if (itemType == ItemType.FirearmWeapon || itemType == ItemType.Cloth)
            {
                newSpawnedSubPanels.Add(GetSubPanel<ItemParametersSubPanel>(invCellData.itemData,
                    (ItemParametersSubPanel)_itemParametersSubPanel));
            }
            
            if (_spawnedSubPanels != null && _spawnedSubPanels.Count != 0)
            {
                foreach (var subPanel in _spawnedSubPanels)
                {
                    subPanel.gameObject.SetActive(false);
                    Destroy(subPanel.gameObject);
                }
            }
            _spawnedSubPanels = newSpawnedSubPanels;
            
            gameObject.SetActive(true);
            
            SetPanelHeight();
            SetPanelPosition(itemCellPosition);
        }

        public void HideInfoPanel()
        {
            gameObject.SetActive(false);
        }

        private void SetPanelPosition(Vector3 elementPosition)
        {
            Vector3 pivot = Vector3.zero;
            
            int halfScreenWidth = Screen.width / 2;
            int halfScreenHeight = Screen.height / 2;
            pivot.x = (elementPosition.x > halfScreenWidth) ? 1 : 0;
            pivot.y = (elementPosition.y > halfScreenHeight) ?  0 : 1;

            informationPanel.pivot = pivot;
            informationPanel.position = elementPosition;
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
            if (_spawnedSubPanels == null
                || _spawnedSubPanels.Count == 0)
                return null;

            foreach (var subPanel in _spawnedSubPanels)
            {
                if (subPanel is T)
                {
                    _spawnedSubPanels.Remove(subPanel);
                    return subPanel;
                }
            }

            return null;
        }

        private void SetPanelHeight()
        {
            float contentMathf = addedHeight;
            foreach (var panel in _spawnedSubPanels)
                contentMathf += panel.RectTransform.rect.height;
            
            informationPanel.sizeDelta = new Vector2 (informationPanel.sizeDelta.x, (contentMathf > maxHeightPanel) ? maxHeightPanel : contentMathf);
        }
    }
}