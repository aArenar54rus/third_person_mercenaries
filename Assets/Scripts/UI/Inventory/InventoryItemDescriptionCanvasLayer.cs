using Arenar.UI;
using UnityEngine;


namespace Arenar.Services.UI
{
    public class InventoryItemDescriptionCanvasLayer : CanvasWindowLayer
    {
        [SerializeField]
        private ItemInformationPanelControlWithButtons mainItemInformationPanelControl;
        [SerializeField]
        private ItemInformationPanelControl secondItemInformationPanelControl;


        public ItemInformationPanelControlWithButtons MainItemInformationPanelControl =>
            mainItemInformationPanelControl;
        
        public ItemInformationPanelControl SecondItemInformationPanelControl =>
            secondItemInformationPanelControl;
    }
}