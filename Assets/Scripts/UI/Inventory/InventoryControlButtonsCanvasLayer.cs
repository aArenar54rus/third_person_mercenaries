using UnityEngine;
using UnityEngine.UI;


namespace Arenar.Services.UI
{
    public class InventoryControlButtonsCanvasLayer : CanvasWindowLayer
    {
        [SerializeField] private Button _backButton;


        public Button BackButton => _backButton;
    }
}