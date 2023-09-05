using UnityEngine;
using UnityEngine.UI;


namespace Arenar.Services.UI
{
    public class InventoryBagCanvasLayer : CanvasWindowLayer
    {
        [SerializeField] private InventoryBagCellController[] inventoryCells;
        [SerializeField] private Slider massSlider;


        public InventoryBagCellController[] InventoryCells => inventoryCells;
        public Slider MassSlider => massSlider;
    }
}