using System.Collections.Generic;
using Arenar.Services.InventoryService;
using TMPro;
using UnityEngine;


namespace Arenar.Services.UI
{
    public class CompleteLevelItemRewardsVisualControl : MonoBehaviour
    {
        [SerializeField] private RectTransform _itemsContainer;
        [SerializeField] private TMP_Text _emptyRewardsText;
        [SerializeField] private InventoryBagCellController _inventoryBagCellPrefab;

        private List<InventoryBagCellController> _inventoryBags = new List<InventoryBagCellController>();


        public void ClearItemRewards()
        {
            foreach (var invBag in _inventoryBags)
            {
                invBag.SetEmpty();
                invBag.gameObject.SetActive(false);
            }
            
            _emptyRewardsText.gameObject.SetActive(true);
        }

        public void AddItem(InventoryItemCellData invItemCellData)
        {
            InventoryBagCellController bag = null;
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
            bag.SetItem(invItemCellData);
            
            _emptyRewardsText.gameObject.SetActive(false);
        }
    }
}