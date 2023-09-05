using UnityEngine;


namespace Arenar.Services.UI
{
    public class InventoryItemDescriptionCanvasLayer : CanvasWindowLayer
    {
        [SerializeField] private ItemInformationPanelControl mainItemInformationPanelControl;
        [SerializeField] private ItemInformationPanelControl secondItemInformationPanelControl;


        public ItemInformationPanelControl MainItemInformationPanelControl =>
            mainItemInformationPanelControl;
        
        public ItemInformationPanelControl SecondItemInformationPanelControl =>
            secondItemInformationPanelControl;
    }
}