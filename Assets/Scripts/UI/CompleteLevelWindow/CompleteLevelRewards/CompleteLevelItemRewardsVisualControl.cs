using System.Collections.Generic;
using Arenar.Services.InventoryService;
using TMPro;
using UnityEngine;

namespace Arenar.Services.UI
{
    public class CompleteLevelItemRewardsVisualControl : MonoBehaviour
    {
        [SerializeField] private RectTransform _itemsContainer;
        [SerializeField] private TMP_Text _xpRewardText;
        [SerializeField] private InventoryBagCellVisual _inventoryBagCellPrefab;

        private List<InventoryBagCellVisual> _inventoryBags = new List<InventoryBagCellVisual>();


        public void ClearItemRewards()
        {
            foreach (var invBag in _inventoryBags)
            {
                invBag.SetEmpty();
                invBag.gameObject.SetActive(false);
            }
        }

        public void AddItem(int cellIndex, InventoryCellData invCellData)
        {
            InventoryBagCellVisual bag = null;
            foreach (var invBag in _inventoryBags)
            {
                if (invBag.gameObject.activeSelf)
                    continue;

                bag = invBag;
                break;
            }

            if (bag == null)
            {
                bag = GameObject.Instantiate(_inventoryBagCellPrefab, _itemsContainer);
                _inventoryBags.Add(bag);
            }

            bag.gameObject.SetActive(true);
            bag.Initialize(cellIndex, invCellData);
        }

        public void SetXpValue(int xpValue)
        {
            _xpRewardText.text = xpValue.ToString();
        }
    }
}