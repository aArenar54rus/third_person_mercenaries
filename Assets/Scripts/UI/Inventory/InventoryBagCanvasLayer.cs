using UnityEngine;
using UnityEngine.UI;


namespace Arenar.Services.UI
{
    public class InventoryBagCanvasLayer : CanvasWindowLayer
    {
        [SerializeField]
        private RectTransform inventoryBagCellParent = null;
        [SerializeField]
        private InventoryBagCellVisual inventoryBagCellPrefab;
        [SerializeField]
        private Slider massSlider;


        public RectTransform InventoryBagCellParent => inventoryBagCellParent;
        public InventoryBagCellVisual InventoryBagCellPrefab => inventoryBagCellPrefab;
        public Slider MassSlider => massSlider;
    }
}